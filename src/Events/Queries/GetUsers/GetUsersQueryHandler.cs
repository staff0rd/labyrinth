using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<PagedResult<User>>>
    {
        private readonly IMediator _mediator;
        private readonly Store _store;

        public GetUsersQueryHandler(IMediator mediator, Store store)
        {
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<PagedResult<User>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<PagedResult<User>> { IsError = true, Message = auth.Message};
            }
            
            var users = _store.GetUsers(request.SourceId);
            var search = request.Search;
            if (!string.IsNullOrWhiteSpace(search))
            {
                Func<string, string> ifNull = (string a) => a == null ? "" : a;
                users = users.Where(u => ifNull(u.Name).ToLower().Contains(search.ToLower()) || ifNull(u.Description).ToLower().Contains(search.ToLower())).ToArray();
            }

            return new Result<PagedResult<User>>(users.GetPagedResult<User>(request.PageNumber, request.PageSize));
        }
    }
}