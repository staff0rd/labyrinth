using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetMessagesQueryHandler : IRequestHandler<GetMessagesQuery, Result<PagedResult<Message>>>
    {
        private readonly IMediator _mediator;
        private readonly Store _store;

        public GetMessagesQueryHandler(IMediator mediator, Store store)
        {
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<PagedResult<Message>>> Handle(GetMessagesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<PagedResult<Message>> { IsError = true, Message = auth.Message};
            }
            
            var messages = _store.GetMessages(request.SourceId);
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