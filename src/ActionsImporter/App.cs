using System.Collections.Immutable;
using ActionsImporter.Interfaces;
using ActionsImporter.Models;

namespace ActionsImporter;

public class App
{
    private const string ActionsImporterImage = "actions-importer/cli";
    private const string DefaultImageRegistry = "ghcr.io";
    private const string CliImageEnvironmentVariable = "GITHUB_ACTIONS_IMPORTER_CLI_IMAGE";

    private readonly IDockerService _dockerService;
    private readonly IProcessService _processService;
    private readonly IConfigurationService _configurationService;

    public bool IsPrerelease { get; set; }
    public bool NoHostNetwork { get; set; }

    private readonly ImmutableDictionary<string, string> _environmentVariables;
    private string ImageTag => IsPrerelease ? "pre" : "latest";

    private string ImageName => $"{ActionsImporterImage}:{ImageTag}";
    private readonly string ActionsImporterContainerRegistry;

    public App(IDockerService dockerService, IProcessService processService, IConfigurationService configurationService, ImmutableDictionary<string, string> environmentVariables)
    {
        _dockerService = dockerService;
        _processService = processService;
        _configurationService = configurationService;
        _environmentVariables = environmentVariables;
        ActionsImporterContainerRegistry = _environmentVariables.TryGetValue("CONTAINER_REGISTRY", out var registry) ? registry : DefaultImageRegistry;
    }

    /// <summary>
    /// Gets the full Docker image name with registry and tag.
    /// Priority: GITHUB_ACTIONS_IMPORTER_CLI_IMAGE env var > CONTAINER_REGISTRY/actions-importer/cli:tag > default
    /// </summary>
    private string GetFullImageName()
    {
        if (_environmentVariables.TryGetValue(CliImageEnvironmentVariable, out var customImage) && !string.IsNullOrWhiteSpace(customImage))
        {
            return customImage;
        }

        return $"{ActionsImporterContainerRegistry}/{ImageName}";
    }

    public async Task<int> UpdateActionsImporterAsync()
    {
        await _dockerService.VerifyDockerRunningAsync().ConfigureAwait(false);

        // If using custom image via env var, pull that; otherwise use default logic
        if (_environmentVariables.TryGetValue(CliImageEnvironmentVariable, out var customImage) && !string.IsNullOrWhiteSpace(customImage))
        {
            await _dockerService.UpdateImageAsync(customImage);
        }
        else
        {
            await _dockerService.UpdateImageAsync(
                ActionsImporterImage,
                ActionsImporterContainerRegistry,
                ImageTag
            );
        }

        return 0;
    }

    public async Task<int> ExecuteActionsImporterAsync(string[] args)
    {
        await _dockerService.VerifyDockerRunningAsync().ConfigureAwait(false);

        // If using custom image via env var, verify and execute with that; otherwise use default logic
        if (_environmentVariables.TryGetValue(CliImageEnvironmentVariable, out var customImage) && !string.IsNullOrWhiteSpace(customImage))
        {
            await _dockerService.VerifyImagePresentAsync(customImage, IsPrerelease).ConfigureAwait(false);
            await _dockerService.ExecuteCommandAsync(
                customImage,
                NoHostNetwork,
                args.Select(x => x.EscapeIfNeeded()).ToArray()
            );
        }
        else
        {
            await _dockerService.VerifyImagePresentAsync(
                ActionsImporterImage,
                ActionsImporterContainerRegistry,
                ImageTag,
                IsPrerelease
            ).ConfigureAwait(false);

            await _dockerService.ExecuteCommandAsync(
                ActionsImporterImage,
                ActionsImporterContainerRegistry,
                ImageTag,
                NoHostNetwork,
                args.Select(x => x.EscapeIfNeeded()).ToArray()
            );
        }
        return 0;
    }

    public async Task<int> GetVersionAsync()
    {
        var (standardOutput, standardError, exitCode) = await _processService.RunAndCaptureAsync("gh", "version");
        var ghActionsImporterVersion = await _processService.RunAndCaptureAsync("gh", "extension list");
        var fullImageName = GetFullImageName();
        var actionsImporterVersion = await _processService.RunAndCaptureAsync("docker", $"run --rm {fullImageName} version", throwOnError: false);

        var formattedGhVersion = standardOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
        var formattedGhActionsImporterVersion = ghActionsImporterVersion.standardOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .FirstOrDefault(x => x.Contains("github/gh-actions-importer", StringComparison.Ordinal));
        var formattedActionsImporterVersion = actionsImporterVersion.standardOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault() ?? "unknown";

        Console.WriteLine(formattedGhVersion);
        Console.WriteLine(formattedGhActionsImporterVersion);
        Console.WriteLine($"{fullImageName}\t{formattedActionsImporterVersion}");

        return 0;
    }

    public async Task CheckForUpdatesAsync()
    {
        try
        {
            var fullImageName = GetFullImageName();
            var latestImageDigestTask = _dockerService.GetLatestImageDigestAsync(fullImageName);
            var currentImageDigestTask = _dockerService.GetCurrentImageDigestAsync(fullImageName);

            await Task.WhenAll(latestImageDigestTask, currentImageDigestTask);

            var latestImageDigest = await latestImageDigestTask;
            var currentImageDigest = await currentImageDigestTask;

            if (latestImageDigest != null && currentImageDigest != null && !latestImageDigest.Equals(currentImageDigest, StringComparison.Ordinal))
            {
                Console.WriteLine("A new version of GitHub Actions Importer is available. Run 'gh actions-importer update' to update.");
            }
        }
        catch (Exception)
        {
            // Let's catch and ignore any exceptions here. We don't want to kill the importer if we failed to check for updates
            // We can add reporting here in the future to alert us of any issues
        }
    }

    public async Task<int> ConfigureAsync(string[] args)
    {
        ImmutableDictionary<string, string>? newVariables;

        if (args.Contains($"--{Commands.Configure.OptionalFeaturesOption.Name}"))
        {
            await _dockerService.VerifyDockerRunningAsync().ConfigureAwait(false);

            // If using custom image via env var, get features from that; otherwise use default logic
            List<Feature> availableFeatures;
            if (_environmentVariables.TryGetValue(CliImageEnvironmentVariable, out var customImage) && !string.IsNullOrWhiteSpace(customImage))
            {
                availableFeatures = await _dockerService.GetFeaturesAsync(customImage).ConfigureAwait(false);
            }
            else
            {
                availableFeatures = await _dockerService.GetFeaturesAsync(ActionsImporterImage, ActionsImporterContainerRegistry, ImageTag).ConfigureAwait(false);
            }

            try
            {
                newVariables = _configurationService.GetFeaturesInput(availableFeatures);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
                return 1;
            }
        }
        else
        {
            newVariables = _configurationService.GetUserInput();
        }

        var mergedVariables = _configurationService.MergeVariables(_environmentVariables, newVariables);
        await _configurationService.WriteVariablesAsync(mergedVariables);

        await Console.Out.WriteLineAsync("Environment variables successfully updated.");
        return 0;
    }
}
