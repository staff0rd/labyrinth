using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Events
{
    public class LinkedInProcessCommandHandler : IRequestHandler<LinkedInProcessCommand>
    {
        private readonly ILogger<LinkedInProcessCommandHandler> _logger;
        private readonly CredentialCache _credentials;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;

        public LinkedInProcessCommandHandler(
            ILogger<LinkedInProcessCommandHandler> logger, CredentialCache credentials, Store store,
            EventRepository events, IProgress progress)
        {
            _logger = logger;
            _credentials = credentials;
            _store = store;
            _events = events;
            _progress = progress;
        }

        public async Task<Unit> Handle(LinkedInProcessCommand request, CancellationToken cancellationToken)
        {
            // if (!_store.IsHydrated)
            //     throw new Exception("Store must be hydrated first");

            var creds = _credentials.Get(Network.LinkedIn, request.Username);

            var count = await _events.GetCount(creds.Username, Network.LinkedIn, "JsonPayload");

            var currentCount = 0;
                
            await _events.ReadForward(creds.Username, creds.Password, Network.LinkedIn, async (events) => { 
                var bodies = events
                    .Where(p => p.EventName == "JsonPayload")
                    .Select(p => p.Body)
                    .ToList();
                foreach (var body in bodies)
                {
                    currentCount++;
                    _progress.Set(currentCount, count);

                    var payload = JsonConvert.DeserializeObject<JsonPayload>(body);

                    var uri = new Uri($"https://linkedin.com{payload.Url}");
                    _logger.LogInformation(uri.LocalPath);
                    
                }
                return events.Last().Id;
            });

            _logger.LogInformation("Completed processing");
            return Unit.Value;
        }

        // public async Task ProcessUser(Rest.LinkedIn.User user, Credential creds)
        // {
        //     var received = Events.User.From(user);
        //     var existing = _store.GetUser(Network.LinkedIn, received.Id);
        //     if (existing == null)
        //     {
        //         _store.Add(Network.LinkedIn, received);
        //     } 
        //     await _events.Sync(creds.Username, creds.Password, Network.LinkedIn, received, existing, _logger, new string[] {});
        // }
    }
}