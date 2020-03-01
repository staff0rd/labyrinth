using MediatR;

namespace Events
{
    public class AuthorizeQuery : IRequest<Result>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}