using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class OverviewQueryHandler : IRequestHandler<OverviewQuery, Result<Overview>>
    {
        private readonly KeyRepository _keys;
        private readonly IMediator _mediator;
        private readonly Store _store;

        public OverviewQueryHandler(IMediator mediator, KeyRepository keys, Store store)
        {
            _keys = keys;
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<Overview>> Handle(OverviewQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<Overview> { IsError = true, Message = auth.Message};
            }

            var result = _store.GetEntityOverview().FirstOrDefault(p => p.SourceId == request.SourceId);

            if (result == null)
                return new Result<Overview> { IsError = true, Message = $"No store initialised for {request.SourceId}" };

            var eventTypes = await _mediator.Send(new GetEventTypesQuery {
                Password = request.Password,
                Username = request.Username,
                SourceId = request.SourceId,
            });

            if (eventTypes.IsError)
                return new Result<Overview> { IsError = true, Message = eventTypes.Message};
            
            result.Events = eventTypes.Response;

            return new Result<Overview>(result);
        }
    }
}