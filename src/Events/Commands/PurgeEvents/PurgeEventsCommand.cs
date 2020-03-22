using System;
using MediatR;

namespace Events
{
    public class PurgeEventsCommand : IRequest
    {
        public string[] Events { get; set; }

        public Guid SourceId { get; set; }

        public string Username { get; set; }
    }
}