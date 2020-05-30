using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class CreateAccountCommandHandler : IRequestHandler<CreateAccountCommand, Result>
    {
        const string USERNAME_ALLOWED_CHARACTERS = @"^\w+$";
        private readonly KeyRepository _keys;

        public CreateAccountCommandHandler(KeyRepository keys) 
        {
            _keys = keys;
        }

        public async Task<Result> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
        {
            var allowedCharacters = new Regex(USERNAME_ALLOWED_CHARACTERS).IsMatch(request.Username);
            if (!allowedCharacters) {
                return Result.Error("Bad username");
            }

            if (await _keys.Exists(request.Username)) {
                return Result.Error($"Username already exists");
            }

            await _keys.Create(request.Username, request.Password);

            return Result.Ok();
        }
    }
}