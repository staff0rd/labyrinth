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
        private readonly Store _store;

        public YammerProcessCommandHandler(ILogger<YammerProcessCommandHandler> logger, CredentialCache credentials, Store store,
            EventRepository events)
        {
            _logger = logger;
            _credentials = credentials;
            _store = store;
            _events = events;
        }

        public async Task<Unit> Handle(YammerProcessCommand request, CancellationToken cancellationToken)
        {
            var creds = _credentials.Yammer[request.Username];
            await _store.Hydrate(creds.Username, creds.Password);

            await _events.ReadForward(creds.Username, creds.Password, Network.Yammer, async (events) => { 
                var bodies = events
                    .Where(p => p.EventName == "RestApiRequest")
                    .Select(p => p.Body)
                    .ToList();
                foreach (var body in bodies)
                {
                    dynamic json = JsonConvert.DeserializeObject(body);

                    foreach(var message in json.response.messages) {
                        await ProcessMessage(Rest.Yammer.Message.FromJson(message.ToString()), creds);
                    }
                    
                    foreach (var reference in json.response.references) {
                        switch(reference.type.ToString()) {
                            case "user": await ProcessUser(Rest.Yammer.User.FromJson(reference.ToString()), creds);
                            break;
                            case "message": await ProcessMessage(Rest.Yammer.Message.FromJson(reference.ToString()), creds);
                            break;
                            default: 
                                _logger.LogWarning($"Unknown reference type: {reference.type}");
                            break;
                        }
                    }
                }
            });
            return Unit.Value;
        }

        public async Task ProcessUser(Rest.Yammer.User user, YammerCredential creds)
        {
            var received = Events.User.From(user);
            var existing = _store.GetUser(Network.Yammer, received.Id);
            if (existing == null)
            {
                _store.Add(Network.Yammer, received);
            } 
            await _events.Sync(creds.Username, creds.Password, Network.Yammer, received, existing, _logger, new string[] {});
        }

        public async Task ProcessMessage(Rest.Yammer.Message message, YammerCredential creds)
        {
            var received = Events.Message.From(message);
            var existing = _store.GetMessage(Network.Yammer, received.Id);
            if (existing == null)
            {
                _store.Add(Network.Yammer, received);
            } 
            await _events.Sync(creds.Username, creds.Password, Network.Yammer, received, existing, _logger, new [] { "BodyParsed" });
        }
    }
}