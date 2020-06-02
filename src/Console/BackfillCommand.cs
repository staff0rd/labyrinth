using McMaster.Extensions.CommandLineUtils;
using Events;

namespace Console
{
    [Command(Description="Backfill commands")]
    [Subcommand(typeof(YammerBackfillCommand), typeof(TeamsBackfillCommand))]
    public class BackfillCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}