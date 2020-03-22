using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class AddSourceCommandHandler : IRequestHandler<AddSourceCommand, Result>
    {
        private SourceRepository _sources;

        public AddSourceCommandHandler(SourceRepository sources)
        {
            _sources = sources;
        }

        public async Task<Result> Handle(AddSourceCommand request, CancellationToken cancellationToken)
        {
            await _sources.Add(new Credential { Username = request.Username, Password = request.Password }, request.Id, request.Network, request.Name);
            return Result.Ok();
        }
    }
}