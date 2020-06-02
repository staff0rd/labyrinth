using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace Events
{
    [Command(Name="purge", Description="Purge all data from source")]
    public class PurgeSourceCommand : ResultCommand
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }

        [Required]
        [Option("-s|--source", CommandOptionType.SingleValue, Description="Source id")]
        public Guid SourceId { get; set; }
    }
}