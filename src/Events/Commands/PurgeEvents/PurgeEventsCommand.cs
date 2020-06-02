using System;
using MediatR;

namespace Events
{
    public class PurgeEventsCommand : ResultCommand
    {
        public string[] Events { get; set; }

        public Guid SourceId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}