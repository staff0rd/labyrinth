using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events.Yammer;
using MediatR;

namespace Events
{
    public class YammerOverviewQueryHandler : IRequestHandler<YammerOverviewQuery, Result<Overview>>
    {
        private readonly KeyRepository _keys;
        private readonly IMediator _mediator;
        private readonly Store _store;

        public YammerOverviewQueryHandler(IMediator mediator, KeyRepository keys, Store store)
        {
            _keys = keys;
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<Overview>> Handle(YammerOverviewQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<Overview> { IsError = true, Message = auth.Message};
            }
            return new Result<Overview>(_store.GetOverview().FirstOrDefault(p => p.Network == Network.Yammer));
        }
    }
}