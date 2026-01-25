using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using ActionsImporter.Interfaces;
using Moq;
using NUnit.Framework;

namespace ActionsImporter.UnitTests;

[TestFixture]
public class AppTests
{
#pragma warning disable CS8618
    private Mock<IProcessService> _processService;
    private Mock<IDockerService> _dockerService;
    private Mock<IConfigurationService> _configurationService;
    private App _app;
    private TextWriter _out;
#pragma warning restore CS8618

    [SetUp]
    public void BeforeEachTest()
    {
        _dockerService = new Mock<IDockerService>();
        _processService = new Mock<IProcessService>();
        _configurationService = new Mock<IConfigurationService>();
        _app = new App(_dockerService.Object, _processService.Object, _configurationService.Object, ImmutableDictionary<string, string>.Empty);
        _out = Console.Out;
    }

    [TearDown]
    public void AfterEachTest()
    {
        Console.SetOut(_out);
    }

    [TestCase("4256ea72fd01deac3e967f6b19f907587dcd6f0a976301f1aecc73dc6f146a4a", "4256ea72fd01deac3e967f6b19f907587dcd6f0a976301f1aecc73dc6f146a4a", "")]
    [TestCase("4256ea72fd01deac3e967f6b19f907587dcd6f0a976301f1aecc73dc6f146a4a", "67eed1493c461efd993be9777598a456562f4e0c6b0bddcb19d819220a06dd4b", "A new version of GitHub Actions Importer is available. Run 'gh actions-importer update' to update.")]
    public async Task CheckForUpdates_NoUpdatesNeeded(string? latestImage, string? currentImage, string result)
    {
        // Arrange
        var fullImageName = "ghcr.io/actions-importer/cli:latest";

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _dockerService.Setup(handler =>
            handler.GetLatestImageDigestAsync(fullImageName)
        ).ReturnsAsync(latestImage);

        _dockerService.Setup(handler =>
            handler.GetCurrentImageDigestAsync(fullImageName)
        ).ReturnsAsync(currentImage);

        // Act
        await _app.CheckForUpdatesAsync();

        // Assert
        Assert.AreEqual(result, stringWriter.ToString().ReplaceLineEndings(""));
        _processService.VerifyAll();
    }

    [Test]
    public async Task CheckForUpdates_RaisesCaughtException()
    {
        // Arrange
        var fullImageName = "ghcr.io/actions-importer/cli:latest";

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _dockerService.Setup(handler =>
            handler.GetLatestImageDigestAsync(fullImageName)
        ).ThrowsAsync(new Exception());

        // Act
        await _app.CheckForUpdatesAsync();

        // Assert
        Assert.AreEqual("", stringWriter.ToString());
        _processService.VerifyAll();
    }

    [Test]
    public async Task CheckForUpdates_WithCustomImage_UsesCustomImage()
    {
        // Arrange
        var customImage = "ghcr.io/robpitcher/actions-importer-cli:latest";
        var envVars = ImmutableDictionary<string, string>.Empty.Add("GITHUB_ACTIONS_IMPORTER_CLI_IMAGE", customImage);
        var app = new App(_dockerService.Object, _processService.Object, _configurationService.Object, envVars);

        using var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        _dockerService.Setup(handler =>
            handler.GetLatestImageDigestAsync(customImage)
        ).ReturnsAsync("4256ea72fd01deac3e967f6b19f907587dcd6f0a976301f1aecc73dc6f146a4a");

        _dockerService.Setup(handler =>
            handler.GetCurrentImageDigestAsync(customImage)
        ).ReturnsAsync("4256ea72fd01deac3e967f6b19f907587dcd6f0a976301f1aecc73dc6f146a4a");

        // Act
        await app.CheckForUpdatesAsync();

        // Assert
        Assert.AreEqual("", stringWriter.ToString());
        _dockerService.VerifyAll();
    }

    [Test]
    public async Task UpdateActionsImporter_WithDefaultImage_UsesDefaultImage()
    {
        // Arrange
        _dockerService.Setup(handler => handler.VerifyDockerRunningAsync()).Returns(Task.CompletedTask);
        _dockerService.Setup(handler =>
            handler.UpdateImageAsync("actions-importer/cli", "ghcr.io", "latest")
        ).Returns(Task.CompletedTask);

        // Act
        var result = await _app.UpdateActionsImporterAsync();

        // Assert
        Assert.AreEqual(0, result);
        _dockerService.VerifyAll();
    }

    [Test]
    public async Task UpdateActionsImporter_WithCustomImage_UsesCustomImage()
    {
        // Arrange
        var customImage = "ghcr.io/robpitcher/actions-importer-cli:latest";
        var envVars = ImmutableDictionary<string, string>.Empty.Add("GITHUB_ACTIONS_IMPORTER_CLI_IMAGE", customImage);
        var app = new App(_dockerService.Object, _processService.Object, _configurationService.Object, envVars);

        _dockerService.Setup(handler => handler.VerifyDockerRunningAsync()).Returns(Task.CompletedTask);
        _dockerService.Setup(handler =>
            handler.UpdateImageAsync(customImage)
        ).Returns(Task.CompletedTask);

        // Act
        var result = await app.UpdateActionsImporterAsync();

        // Assert
        Assert.AreEqual(0, result);
        _dockerService.VerifyAll();
    }
}
