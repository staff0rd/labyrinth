using MediatR;

namespace Events
{
    public class PurgeEventsCommand : IRequest
    {
        public string[] Events { get; set; }

        public Network Network { get; set; }

        public string Username { get; set; }
    }
}