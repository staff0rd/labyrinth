using System.Linq;
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
            var creds = new Credential { Username = request.Username, Password = request.Password };
            var sources = await _sources.Get(creds);
            
            if (sources.Any(s => s.Network == request.Network && s.Name == request.Name))
            {
                return Result.Error($"A source called {request.Name} already exists for ${request.Network}");
            }

            await _sources.Add(creds, request.Id, request.Network, request.Name);
            return Result.Ok();
        }
    }
}