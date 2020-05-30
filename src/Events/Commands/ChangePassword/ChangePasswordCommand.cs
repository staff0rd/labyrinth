using McMaster.Extensions.CommandLineUtils;
using System.ComponentModel.DataAnnotations;

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
}