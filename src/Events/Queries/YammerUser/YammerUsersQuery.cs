using System;
using MediatR;

namespace Events
{
    public class YammerUserQuery : IRequest<Result<User>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Id { get; set; }
    }
}