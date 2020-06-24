using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;

namespace Events.TeamsWeb
{

    [Command(Name="chats", Description="List chats")]
    public class GetChatsQuery : ResultCommand<ChatOverview[]>
    {
        [Required]
        [Option("-t|--token", CommandOptionType.SingleValue, Description = "Token")]
        public string Token { get; set; }
        
        protected override void DisplayResult(ChatOverview[] result)
        {
            ConsoleTable
                .From(result)
                .Write(Format.Minimal);
        }
    }
}