using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<User>>
    {
        private readonly IMediator _mediator;
        private readonly Store _store;

        public GetUserQueryHandler(IMediator mediator, Store store)
        {
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<User>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<User> { IsError = true, Message = auth.Message};
            }
            var user = _store.GetUsers(request.SourceId).FirstOrDefault(p => p.Id == request.Id);
            return new Result<User>(user);
        }
    }
}