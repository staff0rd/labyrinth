using System.ComponentModel.DataAnnotations;
using System.Linq;
using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;

namespace Events.TeamsWeb
{
    [Command(Name="users", Description="Get Users by Id")]
    public class GetUsersQuery : ResultCommand<FetchShortProfile>
    {
        [Required]
        [Option("-t|--token", CommandOptionType.SingleValue, Description = "Token")]
        public string Token { get; set; }

        [Required]
        [Option("-u|--user", CommandOptionType.MultipleValue, Description="User Ids")]
        public string[] UserIds { get; set; }
        
        protected override void DisplayResult(FetchShortProfile result)
        {
            var users = result.Value
                .Select(p => new {
                    p.DisplayName,
                    p.Email,
                    p.JobTitle,
                    p.UserLocation,
                    p.UserType,
                });

            ConsoleTable
                .From(users)
                .Write(Format.Minimal);
        }
    }
}