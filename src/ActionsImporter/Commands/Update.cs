using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace ActionsImporter.Commands;

public class Update : BaseCommand
{
    protected override string Name => "update";
    protected override string Description => "Update to the latest version of GitHub Actions Importer. Use GITHUB_ACTIONS_IMPORTER_CLI_IMAGE environment variable to specify a custom Docker image.";

    protected override Command GenerateCommand(App app)
    {
        var command = base.GenerateCommand(app);

        command.AppendPrereleaseOption();

        command.Handler = CommandHandler.Create(() => app.UpdateActionsImporterAsync());

        return command;
    }
}
