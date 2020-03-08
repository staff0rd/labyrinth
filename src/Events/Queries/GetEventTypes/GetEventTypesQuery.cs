using MediatR;

namespace Events
{
    public class GetEventTypesQuery : IRequest<Result<EventCount[]>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Network Network { get; set; }
    }
}