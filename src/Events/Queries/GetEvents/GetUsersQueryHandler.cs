using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetEventsQueryHandler : IRequestHandler<GetEventsQuery, Result<NextPagedResult<Event>>>
    {
        private readonly IMediator _mediator;
        private readonly EventRepository _events;

        public GetEventsQueryHandler(IMediator mediator, EventRepository events)
        {
            _mediator = mediator;
            _events = events;
        }

        public async Task<Result<NextPagedResult<Event>>> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<NextPagedResult<Event>> { IsError = true, Message = auth.Message};
            }

            var events = await _events.Paginate(request.Username, request.Password, request.Network, request.LastId, request.PageSize);
            var totalRows = await _events.GetCount(request.Username, request.Network, request.EventTypes);

            return new Result<NextPagedResult<Event>>(new NextPagedResult<Event> {
                LastId = events.Rows.Last().Id,
                PageSize = request.PageSize,
                Rows = events.Rows.ToArray(),
                TotalRows = totalRows,
            });
        }
    }
}