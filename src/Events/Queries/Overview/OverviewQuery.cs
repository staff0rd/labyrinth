using MediatR;

namespace Events
{
    public class OverviewQuery : IRequest<Result<Overview>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Network Network { get; set; }
    }
}