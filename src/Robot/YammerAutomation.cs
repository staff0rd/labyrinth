using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Events;
using Events.Yammer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest.Yammer;

namespace Robot
{
    public class YammerAutomation {
        private readonly ILogger _logger;
        private readonly EventRepository _events;
        private readonly RestEventManager _rest;
        private readonly Store _store;
        private readonly string _token;

        public YammerAutomation(ILogger logger, EventRepository events)
            : this(logger, events, null) {}

        public YammerAutomation(ILogger logger, EventRepository events, string token) {
            _logger = logger;
            _events = events;
            _rest = new RestEventManager(logger, events);
            _store = new Store(_events, _logger);
            _token = token;
        }

        public async Task Process() {
            await _store.Hydrate();

            await _events.ReadForward(Network.Yammer, async (events) => { 
                var bodies = events
                    .Where(p => p.EventName == "RestApiRequest")
                    .Select(p => p.Body)
                    .ToList();
                foreach (var body in bodies)
                {
                    dynamic json = JsonConvert.DeserializeObject(body);

                    foreach(var message in json.response.messages) {
                        await ProcessMessage(Rest.Yammer.Message.FromJson(message.ToString()));
                    }
                    
                    foreach (var reference in json.response.references) {
                        switch(reference.type.ToString()) {
                            case "user": await ProcessUser(Rest.Yammer.User.FromJson(reference.ToString()));
                            break;
                            case "message": await ProcessMessage(Rest.Yammer.Message.FromJson(reference.ToString()));
                            break;
                            default: 
                                _logger.LogWarning($"Unknown reference type: {reference.type}");
                            break;
                        }
                    }
                }
            });
        }

        public async Task ProcessUser(Rest.Yammer.User user)
        {
            var received = Events.User.From(user);
            var existing = _store.GetUser(Network.Yammer, received.Id);
            if (existing == null)
            {
                _store.Add(Network.Yammer, received);
            } 
            await _events.Sync(Network.Yammer, received, existing, _logger, new string[] {});
        }

        public async Task ProcessMessage(Rest.Yammer.Message message)
        {
            var received = Events.Message.From(message);
            var existing = _store.GetMessage(Network.Yammer, received.Id);
            if (existing == null)
            {
                _store.Add(Network.Yammer, received);
            } 
            await _events.Sync(Network.Yammer, received, existing, _logger, new [] { "BodyParsed" });
        }
        
        
    }
}