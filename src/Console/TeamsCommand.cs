using McMaster.Extensions.CommandLineUtils;
using Events;
using Events.TeamsWeb;

namespace Console
{
    [Command(Description="Teams commands")]
    [Subcommand(typeof(GetOverviewQuery))]
    public class TeamsCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}