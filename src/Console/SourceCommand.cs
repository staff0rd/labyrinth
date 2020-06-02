using McMaster.Extensions.CommandLineUtils;
using Events;

namespace Console
{
    [Command(Description="Source commands")]
    [Subcommand(
        typeof(AddSourceCommand),
        typeof(GetSourcesQuery),
        typeof(PurgeSourceCommand)
    )]
    public class SourceCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}