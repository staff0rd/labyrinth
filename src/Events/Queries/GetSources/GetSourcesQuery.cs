using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using MediatR;
using ConsoleTables;

namespace Events
{
    [Command(Name="list", Description="Get the list of sources currently configured")]
    public class GetSourcesQuery : ResultCommand<List<Source>>
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }

        protected override void DisplayResult(List<Source> result)
        {
            ConsoleTable
                .From(result)
                .Write(Format.Minimal);
        }
    }
}