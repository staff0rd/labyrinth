using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace Events
{
    [Command(Name="add", Description="Add a source")]
    public class AddSourceCommand : ResultCommand
    {
        public AddSourceCommand()
        {
            Id = Guid.NewGuid();
        }

        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }

        public Guid Id { get; set; }

        [Required]
        [Option("-s|--source", CommandOptionType.SingleValue, Description="Source name")]
        public string Name { get; set;}

        [Required]
        [Option("-n|--network", CommandOptionType.SingleValue, Description="Network")]
        public Network Network { get; set; }
    }
}