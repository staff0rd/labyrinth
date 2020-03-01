using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Events.Yammer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rest.Yammer;

namespace Events
{
    public class NetworkStore
    {
        public Dictionary<string, Message> Messages { get; set; } = new Dictionary<string, Message>();
        public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();
    }

    public class Store
    {
        private readonly EventRepository _events;
        private readonly ILogger _logger;

        public Dictionary<Network, NetworkStore> _store;

        public Store(EventRepository events, ILogger logger) {
            _events = events;
            _logger = logger;
            _store = new Dictionary<Network, NetworkStore> {
                { Network.Yammer, new NetworkStore() },
                { Network.LinkedIn, new NetworkStore() },
            };
        }

        public Events.Yammer.Overview[] GetOverview() {
            return _store.Select(p => new Overview { Network = p.Key, Messages = p.Value.Messages.Count, Users = p.Value.Users.Count})
                .ToArray();
        }

        public Func<Event[], Task> FillFromEvents<T>(Dictionary<string, T> dictionary) where T : IExternalEntity
        {
            return (events) => {
                var eventName = $"{typeof(T).Name}Created";
                events
                    .Where(p => p.EventName == eventName)
                    .Select(p => JsonConvert.DeserializeObject<T>(p.Body))
                    .ToList()
                    .ForEach(item => {
                        if (!dictionary.ContainsKey(item.Id))
                            dictionary.Add(item.Id, item);
                    });
                return Task.CompletedTask;
            };
        }

        public async Task Hydrate()
        {
            var sw = Stopwatch.StartNew();
           
            await _events.ReadForward(Network.Yammer, FillFromEvents<Message>(_store[Network.Yammer].Messages));
            await _events.ReadForward(Network.Yammer, FillFromEvents<User>(_store[Network.Yammer].Users));
            
            _logger.LogInformation($"It look {sw.Elapsed} to read network {Network.Yammer}, {_store[Network.Yammer].Users.Count} users, {_store[Network.Yammer].Messages.Count} messages");
        }

        public void Add(Network network, User user)
        {
            _store[network].Users.Add(user.Id.ToString(), user);
            _logger.LogInformation($"Added user {user.Id} to {network}");
        }

        public void Add(Network network, Message message)
        {
            _store[network].Messages.Add(message.Id.ToString(), message);
            _logger.LogInformation($"Added message {message.Id} to {Network.Yammer}");
        }

        public User GetUser(Network network, string id)
        {
            _store[network].Users.TryGetValue(id, out var existing);
            return existing;
        }

        public Message GetMessage(Network network, string id)
        {
            _store[network].Messages.TryGetValue(id, out var existing);
            return existing;
        }

        public IEnumerable<Message> GetMessages(Network network) {
            return _store[network].Messages.ToList().Select(x => x.Value);
        }
        public IEnumerable<User> GetUsers(Network network) {
            return _store[network].Users.ToList().Select(x => x.Value);
        }
    }
}