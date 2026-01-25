using ActionsImporter.Models;

namespace ActionsImporter.Interfaces;

public interface IDockerService
{
    Task UpdateImageAsync(string image, string server, string version);
    Task UpdateImageAsync(string fullImageName);

    Task ExecuteCommandAsync(string image, string server, string version, bool noHostNetwork, params string[] arguments);
    Task ExecuteCommandAsync(string fullImageName, bool noHostNetwork, params string[] arguments);

    Task<List<Feature>> GetFeaturesAsync(string image, string server, string version);
    Task<List<Feature>> GetFeaturesAsync(string fullImageName);

    Task VerifyDockerRunningAsync();

    Task VerifyImagePresentAsync(string image, string server, string version, bool isPrerelease);
    Task VerifyImagePresentAsync(string fullImageName, bool isPrerelease);

    Task<string?> GetLatestImageDigestAsync(string image, string server);
    Task<string?> GetLatestImageDigestAsync(string fullImageName);

    Task<string?> GetCurrentImageDigestAsync(string image, string server);
    Task<string?> GetCurrentImageDigestAsync(string fullImageName);
}
