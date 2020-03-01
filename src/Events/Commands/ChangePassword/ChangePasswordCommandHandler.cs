using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly KeyRepository _keys;

        public ChangePasswordCommandHandler(KeyRepository keys)
        {
            _keys = keys;
        }
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var result = await _keys.ChangePassword(request.Username, request.OldPassword, request.NewPassword);
            if (result)
                return Result.Ok();
            else
                return Result.Error("Password change failed");
        }
    }
}