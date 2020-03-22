using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Events
{
    public class YammerProcessCommandHandler : IRequestHandler<YammerProcessCommand>
    {
        private readonly ILogger<YammerProcessCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;

        public YammerProcessCommandHandler(
            ILogger<YammerProcessCommandHandler> logger, CredentialCache credentials, Store store,
            EventRepository events, IProgress progress)
        {
            _logger = logger;
            _credentials = credentials;
            _store = store;
            _events = events;
            _progress = progress;
        }

        public async Task<Unit> Handle(YammerProcessCommand request, CancellationToken cancellationToken)
        {
            if (!_store.IsHydrated)
                throw new Exception("Store must be hydrated first");

            var creds = _credentials.Get(request.SourceId, request.Username);

            var count = await _events.GetCount(creds.Username, request.SourceId, "RestApiRequest");
            var currentCount = 0;
                
            await _events.ReadForward(creds, request.SourceId, count, async (events, totalCount) => { 
                var bodies = events
                    .Where(p => p.EventName == "RestApiRequest")
                    .Select(p => p.Body)
                    .ToList();
                foreach (var body in bodies)
                {
                    currentCount++;
                    _progress.Set(currentCount, totalCount);

                    dynamic json = JsonConvert.DeserializeObject(body);

                    if (json.response != null) {

                        foreach(var message in json.response.messages) {
                            await ProcessMessage(Rest.Yammer.Message.FromJson(message.ToString()), request.SourceId, creds);
                        }
                        foreach (var reference in json.response.references) {
                            switch(reference.type.ToString()) {
                                case "user": await ProcessUser(Rest.Yammer.User.FromJson(reference.ToString()), request.SourceId, creds);
                                break;
                                case "message": await ProcessMessage(Rest.Yammer.Message.FromJson(reference.ToString()), request.SourceId, creds);
                                break;
                                default: 
                                    _logger.LogWarning($"Unknown reference type: {reference.type}");
                                break;
                            }
                        }
                    }
                }
                return events.Last().Id;
            });

            _logger.LogInformation("Completed processing");
            return Unit.Value;
        }

        public async Task ProcessUser(Rest.Yammer.User user, Guid sourceId, Credential creds)
        {
            var received = Events.User.From(user);
            var existing = _store.GetUser(sourceId, received.Id);
            if (existing == null)
            {
                _store.Add(sourceId, received);
            } 
            await _events.Sync(creds, sourceId, received, existing, new string[] {});
        }

        public async Task ProcessMessage(Rest.Yammer.Message message, Guid sourceId, Credential creds)
        {
            var received = Events.Message.From(message);
            var existing = _store.GetMessage(sourceId, received.Id);
            if (existing == null)
            {
                _store.Add(sourceId, received);
            } 
            await _events.Sync(creds, sourceId, received, existing, new [] { "BodyParsed" });
        }
    }
}