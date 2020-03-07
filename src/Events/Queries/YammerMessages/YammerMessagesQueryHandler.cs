using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Events.Yammer;
using MediatR;

namespace Events
{
    public class YammerMessagesQueryHandler : IRequestHandler<YammerMessagesQuery, Result<PagedResult<Message>>>
    {
        private readonly KeyRepository _keys;
        private readonly IMediator _mediator;
        private readonly Store _store;

        public YammerMessagesQueryHandler(IMediator mediator, KeyRepository keys, Store store)
        {
            _keys = keys;
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<PagedResult<Message>>> Handle(YammerMessagesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<PagedResult<Message>> { IsError = true, Message = auth.Message};
            }
            
            var messages = _store.GetMessages(Network.Yammer);
            var search = request.Search;
            if (!string.IsNullOrWhiteSpace(search))
            {
                Func<string, string> ifNull = (string a) => a == null ? "" : a;
                messages = messages.Where(u => ifNull(u.BodyPlain).ToLower().Contains(search.ToLower())).ToArray();
            }

            return new Result<PagedResult<Message>>(messages.GetPagedResult<Message>(request.PageNumber, request.PageSize));
        }
    }
}