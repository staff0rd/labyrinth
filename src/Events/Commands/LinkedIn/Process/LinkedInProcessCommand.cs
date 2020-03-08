using MediatR;

namespace Events
{
    public class LinkedInProcessCommand : IRequest
    {
        public string Username { get; set; }
    }
}