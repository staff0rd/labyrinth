using Events.Yammer;
using MediatR;

namespace Events
{
    public class YammerOverviewQuery : IRequest<Result<Overview>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}