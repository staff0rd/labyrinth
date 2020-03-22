using System;
using MediatR;

namespace Events
{
    public class OverviewQuery : IRequest<Result<Overview>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid SourceId { get; set; }
    }
}