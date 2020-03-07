using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class AuthorizeQueryHandler : IRequestHandler<AuthorizeQuery, Result>
    {
        private readonly KeyRepository _keys;

        public AuthorizeQueryHandler(KeyRepository keys)
        {
            _keys = keys;
        }

        public async Task<Result> Handle(AuthorizeQuery request, CancellationToken cancellationToken)
        {
            var authorized = await _keys.TestPassword(request.Username, request.Password);
            return new Result { IsError = !authorized, Message = authorized ? "Authorized" : "Not authorized" };
        }
    }
}