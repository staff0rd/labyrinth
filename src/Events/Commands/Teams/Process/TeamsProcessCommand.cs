using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using MediatR;

namespace Events
{
    [Command(Name="teams")]
    public class TeamsProcessCommand : ResultCommand
    {
        [Required]
        [Option("-u|--username", CommandOptionType.SingleValue, Description = "User name")]
        public string LabyrinthUsername { get; set; }
        
        [Required]
        [Option("-p|--password", CommandOptionType.SingleValue, Description = "Password")]
        public string LabyrinthPassword { get; set; }

        [Required]
        [Option("-s|--source", CommandOptionType.SingleValue, Description = "Source Id")]
        public Guid SourceId { get; set;}
    }
}