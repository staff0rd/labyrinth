using McMaster.Extensions.CommandLineUtils;
using Events;
using Events.TeamsWeb;

namespace Console
{
    [Command(Description="Teams commands")]
    [Subcommand(typeof(GetChatsQuery), typeof(Events.TeamsWeb.GetUsersQuery))]
    public class TeamsCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}