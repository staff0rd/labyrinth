using System;
using MediatR;

namespace Events
{
    public class AddSourceCommand : IRequest<Result>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set;}
        public Network Network { get; set; }
    }
}