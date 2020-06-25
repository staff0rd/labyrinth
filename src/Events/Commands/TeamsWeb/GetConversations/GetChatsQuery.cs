using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;

namespace Events.TeamsWeb
{

    [Command(Name="chats", Description="List chats")]
    [TokenOrTokenFileRequired]
    public class GetChatsQuery : ResultCommand<ChatOverview[]>, ITokenOrTokenFile
    {
        [Option("-t|--token", CommandOptionType.SingleValue, Description = "Token")]
        public string Token { get; set; }

        [Option("-f|--token-file", CommandOptionType.SingleValue, Description = "Token File")]
        public string TokenFile { get; set; }

        protected override void DisplayResult(ChatOverview[] result)
        {
            ConsoleTable
                .From(result)
                .Write(Format.Minimal);
        }
    }
}