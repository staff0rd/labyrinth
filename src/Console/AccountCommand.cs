using McMaster.Extensions.CommandLineUtils;
using Events;

namespace Console
{
    [Command(Description="Account commands")]
    [Subcommand(typeof(CreateAccountCommand), typeof(ChangePasswordCommand))]
    public class AccountCommand {
        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}