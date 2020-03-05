using MediatR;

namespace Events
{
    public class YammerProcessCommand : IRequest
    {
        public string Username { get; set; }
    }
}