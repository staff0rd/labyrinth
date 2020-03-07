using MediatR;

namespace Events
{
    public class HydrateCommand : IRequest
    {
        public string Username { get; set; }
    }
}