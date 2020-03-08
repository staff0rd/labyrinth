using MediatR;

namespace Events
{
    public class GetEventTypesQuery : IRequest<Result<string[]>>
    {
        public string Username { get; set; }
    }
}