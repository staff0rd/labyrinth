using MediatR;
using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace Events
{
    [Command(Name = "change-password", Description = "Change a user's password")]
    public class ChangePasswordCommand : ResultCommand
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Existing password")]
        public string OldPassword { get; set; }
        
        [Required]
        [Option("-n|--new-password", CommandOptionType.SingleValue, Description = "New password")]
        public string NewPassword { get; set; }
    }

    public abstract class ResultCommand : IRequest<Result>
    {
        protected async Task OnExecuteAsync(IMediator mediator, ILogger<ResultCommand> logger)
        {
            var result = await mediator.Send(this);
            if (result.IsError)
                logger.LogError(result.Message);
            else
                logger.LogInformation("Success");
        }
    }
}