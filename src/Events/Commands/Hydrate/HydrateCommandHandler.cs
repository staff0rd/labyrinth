using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class HydrateCommandHandler : IRequestHandler<HydrateCommand>
    {
        private readonly ILogger<HydrateCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;

        public HydrateCommandHandler(ILogger<HydrateCommandHandler> logger, CredentialCache credentials, Store store,
            EventRepository events, IProgress progress)
        {
            _logger = logger;
            _credentials = credentials;
            _store = store;
            _events = events;
            _progress = progress;
        }

        public async Task<Unit> Handle(HydrateCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(Network.Self, request.Username); 
            await _store.Hydrate(request.Username, credential.Password);
            return Unit.Value;
        }
    }
}