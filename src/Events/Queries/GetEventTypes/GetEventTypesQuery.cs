using System;
using MediatR;

namespace Events
{
    public class GetEventTypesQuery : IRequest<Result<EventCount[]>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid SourceId { get; set; }
    }
}