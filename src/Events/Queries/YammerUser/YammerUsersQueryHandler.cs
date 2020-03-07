using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class YammerUserQueryHandler : IRequestHandler<YammerUserQuery, Result<User>>
    {
        private readonly KeyRepository _keys;
        private readonly IMediator _mediator;
        private readonly Store _store;

        public YammerUserQueryHandler(IMediator mediator, KeyRepository keys, Store store)
        {
            _keys = keys;
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<User>> Handle(YammerUserQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<User> { IsError = true, Message = auth.Message};
            }
            var user = _store.GetUsers(Network.Yammer).FirstOrDefault(p => p.Id == request.Id);
            return new Result<User>(user);
        }
    }
}