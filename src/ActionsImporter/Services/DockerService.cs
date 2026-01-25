using System.Text.Json;
using ActionsImporter.Interfaces;
using ActionsImporter.Models.Docker;
using ActionsImporter.Models;

namespace ActionsImporter.Services;

public class DockerService : IDockerService
{
    private readonly IProcessService _processService;
    private readonly IRuntimeService _runtimeService;

    public DockerService(IProcessService processService, IRuntimeService runtimeService)
    {
        _processService = processService;
        _runtimeService = runtimeService;
    }

    public Task UpdateImageAsync(string image, string server, string version)
    {
        return DockerPullAsync(image, server, version);
    }

    public Task UpdateImageAsync(string fullImageName)
    {
        return DockerPullAsync(fullImageName);
    }

    public async Task ExecuteCommandAsync(string image, string server, string version, bool noHostNetwork, params string[] arguments)
    {
        var fullImageName = $"{server}/{image}:{version}";
        await ExecuteCommandAsync(fullImageName, noHostNetwork, arguments);
    }

    public async Task ExecuteCommandAsync(string fullImageName, bool noHostNetwork, params string[] arguments)
    {
        var actionsImporterArguments = new List<string>
        {
            "run --rm -t"
        };

        if (!noHostNetwork)
        {
            actionsImporterArguments.Add("--network=host");
        }

        actionsImporterArguments.AddRange(GetEnvironmentVariableArguments());

        var dockerArgs = Environment.GetEnvironmentVariable("DOCKER_ARGS");
        if (dockerArgs is not null)
        {
            actionsImporterArguments.Add(dockerArgs);
        }

        // Forward the current user's UID/GID to the container
        // to ensure the output files are owned by the current user
        if (_runtimeService.IsLinux)
        {
            var (userId, _, _) = await _processService.RunAndCaptureAsync("id", "-u");
            var (groupId, _, _) = await _processService.RunAndCaptureAsync("id", "-g");
            actionsImporterArguments.Add($"-e USER_ID={userId.TrimEnd()}");
            actionsImporterArguments.Add($"-e GROUP_ID={groupId.TrimEnd()}");
        }
        actionsImporterArguments.Add($"-v \"{Directory.GetCurrentDirectory()}\":/data");
        actionsImporterArguments.Add(fullImageName);
        actionsImporterArguments.AddRange(arguments);

        await _processService.RunAsync(
            "docker",
            string.Join(' ', actionsImporterArguments),
            Directory.GetCurrentDirectory(),
            new[] { ("MSYS_NO_PATHCONV", "1") }
        );
    }

    public async Task<List<Feature>> GetFeaturesAsync(string image, string server, string version)
    {
        var fullImageName = $"{server}/{image}:{version}";
        return await GetFeaturesAsync(fullImageName);
    }

    public async Task<List<Feature>> GetFeaturesAsync(string fullImageName)
    {
        var actionsImporterArguments = new List<string> { "run --rm -t" };
        actionsImporterArguments.AddRange(GetEnvironmentVariableArguments());
        actionsImporterArguments.Add(fullImageName);
        actionsImporterArguments.AddRange(new[] { "list-features", "--json" });

        var (standardOutput, _, _) = await _processService.RunAndCaptureAsync("docker", string.Join(' ', actionsImporterArguments), throwOnError: false);

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, };
        try
        {
            return JsonSerializer.Deserialize<List<Feature>>(standardOutput, options) ?? new();
        }
        catch (Exception)
        {
            // If unable to get the features from the container, return an empty list
            // An empty list will result in a message being displayed to the user
            return new();
        }
    }

    public async Task VerifyDockerRunningAsync()
    {
        try
        {
            await _processService.RunAsync(
                "docker",
                "info",
                output: false
            );
        }
        catch (Exception)
        {
            throw new Exception("Please ensure docker is installed and the docker daemon is running");
        }
    }

    public async Task VerifyImagePresentAsync(string image, string server, string version, bool isPrerelease)
    {
        var fullImageName = $"{server}/{image}:{version}";
        await VerifyImagePresentAsync(fullImageName, isPrerelease);
    }

    public async Task VerifyImagePresentAsync(string fullImageName, bool isPrerelease)
    {
        var preReleaseOption = isPrerelease ? " --prerelease" : string.Empty;
        try
        {
            await _processService.RunAsync(
                "docker",
                $"image inspect {fullImageName}",
                output: false
            );
        }

        catch (Exception)
        {
            throw new Exception($"Unable to locate {fullImageName} image locally. Please run `gh actions-importer update{preReleaseOption}` to fetch the latest image prior to running this command.");
        }
    }

    public async Task<string?> GetLatestImageDigestAsync(string image, string server)
    {
        var fullImageName = $"{server}/{image}";
        return await GetLatestImageDigestAsync(fullImageName);
    }

    public async Task<string?> GetLatestImageDigestAsync(string fullImageName)
    {
        var (standardOutput, _, _) = await _processService.RunAndCaptureAsync("docker", $"manifest inspect {fullImageName}");
        Manifest? manifest = JsonSerializer.Deserialize<Manifest>(standardOutput);

        return manifest?.GetDigest();
    }

    public async Task<string?> GetCurrentImageDigestAsync(string image, string server)
    {
        var fullImageName = $"{server}/{image}";
        return await GetCurrentImageDigestAsync(fullImageName);
    }

    public async Task<string?> GetCurrentImageDigestAsync(string fullImageName)
    {
        var (standardOutput, _, _) = await _processService.RunAndCaptureAsync("docker", $"image inspect --format={{{{.Id}}}} {fullImageName}");

        return standardOutput.Split(":").ElementAtOrDefault(1)?.Trim();
    }

    private static IEnumerable<string> GetEnvironmentVariableArguments()
    {
        if (File.Exists(".env.local"))
        {
            yield return "--env-file .env.local";
        }

        foreach (var env in Constants.EnvironmentVariables)
        {
            var value = Environment.GetEnvironmentVariable(env);

            if (string.IsNullOrWhiteSpace(value)) continue;

            var key = env;
            if (key.StartsWith("GH_", StringComparison.Ordinal))
                key = key.Replace("GH_", "GITHUB_", StringComparison.Ordinal);

            yield return $"--env \"{key}={value}\"";
        }
    }

    private async Task DockerPullAsync(string image, string server, string version)
    {
        var fullImageName = $"{server}/{image}:{version}";
        await DockerPullAsync(fullImageName);
    }

    private async Task DockerPullAsync(string fullImageName)
    {
        Console.WriteLine($"Updating {fullImageName}...");
        var (_, standardError, exitCode) = await _processService.RunAndCaptureAsync(
            "docker",
            $"pull {fullImageName} --quiet",
            throwOnError: false
        );

        if (exitCode != 0)
        {
            string message = standardError.Trim();
            string errorMessage = $"There was an error pulling the {fullImageName}.\nError: {message}";

            throw new Exception(errorMessage);
        }
        Console.WriteLine($"{fullImageName} up-to-date");
    }
}
