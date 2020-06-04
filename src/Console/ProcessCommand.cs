using McMaster.Extensions.CommandLineUtils;
using Events;

namespace Console
{
    [Command(Description="Process commands")]
    [Subcommand(typeof(TeamsProcessCommand))]
    public class ProcessCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}