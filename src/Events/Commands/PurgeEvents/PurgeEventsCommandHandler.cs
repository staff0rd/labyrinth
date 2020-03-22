using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class PurgeEventsCommandHandler : IRequestHandler<PurgeEventsCommand>
    {
        private readonly ILogger<PurgeEventsCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;

        public PurgeEventsCommandHandler(ILogger<PurgeEventsCommandHandler> logger, CredentialCache credentials, EventRepository events)
        {
            _logger = logger;
            _credentials = credentials;
            _events = events;
        }

        public async Task<Unit> Handle(PurgeEventsCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username); 

            var count = _events.GetCount(request.Username, request.SourceId, request.Events);

            var events = string.Join(", ", request.Events);

            _logger.LogInformation($"Purging {count} {events} events from {request.SourceId}");

            await _events.Delete(request.Username, credential.Password, request.SourceId, request.Events);

            return Unit.Value;
        }
    }
}