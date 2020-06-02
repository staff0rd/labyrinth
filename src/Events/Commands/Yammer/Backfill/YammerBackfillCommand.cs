using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace Events
{
    [Command(Name="yammer", Description="Backfill a Yammer source")]
    public class YammerBackfillCommand : ResultCommand
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string Username { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string Password { get; set; }

        [Required]
        [Option("-s|--source", CommandOptionType.SingleValue, Description = "Source Id")]
        public Guid SourceId { get; set;}
        
        [Required]
        [Option("-t|--token", CommandOptionType.SingleValue, Description = "Token")]
        public string Token { get; set; }
    }
    
}