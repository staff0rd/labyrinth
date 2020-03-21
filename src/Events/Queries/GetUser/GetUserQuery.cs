using System;
using MediatR;

namespace Events
{
    public class GetUserQuery : IRequest<Result<User>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Id { get; set; }
        public Network Network { get; set; }
    }
}