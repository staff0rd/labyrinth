using System;
using MediatR;

namespace Events
{
    public class QueueTeamsProcessCommand : IRequest
    {
        public string Username { get; set; }
        public Guid SourceId { get; set;}
    }
    
}