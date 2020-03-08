using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetEventTypesQueryHandler : IRequestHandler<GetEventTypesQuery, Result<EventCount[]>>
    {
        private readonly IMediator _mediator;
        private readonly EventRepository _events;

        public GetEventTypesQueryHandler(IMediator mediator, EventRepository events) 
        {
            _mediator = mediator;
            _events = events;
        }

        public async Task<Result<EventCount[]>> Handle(GetEventTypesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<EventCount[]> { IsError = true, Message = auth.Message};
            }

            return new Result<EventCount[]>(await _events.GetEventTypes(request.Username, request.Network));
        }
    }
}