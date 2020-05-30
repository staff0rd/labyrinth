using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace Events 
{
    [Command(Name="create-account", Description= "Create a new account")]
    public class CreateAccountCommand : ResultCommand
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }
    }
}