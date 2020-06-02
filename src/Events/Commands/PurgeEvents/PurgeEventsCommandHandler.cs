using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class PurgeEventsCommandHandler : IRequestHandler<PurgeEventsCommand, Result>
    {
        private readonly ILogger<PurgeEventsCommandHandler> _logger;
        private readonly EventRepository _events;
        private readonly KeyRepository _keys;

        public PurgeEventsCommandHandler(ILogger<PurgeEventsCommandHandler> logger, EventRepository events, KeyRepository keys)
        {
            _logger = logger;
            _events = events;
            _keys = keys;
        }

        public async Task<Result> Handle(PurgeEventsCommand request, CancellationToken cancellationToken)
        {
            if (await _keys.TestPassword(request.Username, request.Password))
            {
                var credential = new Credential(request.Username, request.Password);

                var count = await _events.GetCount(request.Username, request.SourceId, request.Events);

                var events = request.Events.Length > 0 ? string.Join(", ", request.Events) : "(all)";

                _logger.LogInformation($"Purging {count} {events} events from {request.SourceId}");

                await _events.Delete(request.Username, credential.Password, request.SourceId, request.Events);

                return Result.Ok();
            } else 
            {
                return Result.Error("Bad password");
            }
        }
    }
}