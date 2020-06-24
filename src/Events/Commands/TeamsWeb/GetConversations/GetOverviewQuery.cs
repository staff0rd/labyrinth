using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;

namespace Events.TeamsWeb
{
    [Command(Name="chats", Description="List chats")]
    public class GetOverviewQuery : ResultCommand<Overview>
    {
        [Required]
        [Option("-t|--token", CommandOptionType.SingleValue, Description = "Token")]
        public string Token { get; set; }
        
        protected override void DisplayResult(Overview result)
        {
            var conversations = result
                .Chats
                .Where(p => p.ChatType == ChatType.Chat)
                .OrderByDescending(p => p.LastMessage.ComposeTime)
                .Select(p => new {
                    Author = p.LastMessage?.ImDisplayName,
                    Time = p.LastMessage?.ComposeTime?.ToLocalTime(),
                    Message= p.LastMessage?.Content
                });

            ConsoleTable
                .From(conversations)
                .Write(Format.Minimal);
        }
    }
}