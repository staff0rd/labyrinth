using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Events
{
    public class Store
    {
        private readonly EventRepository _events;
        private readonly ILogger<Store> _logger;
        private readonly IProgress _progress;

        private bool _isHydrated;
        public bool IsHydrated => _isHydrated;

        public Dictionary<Network, NetworkStore> _store;

        public Store(EventRepository events, ILogger<Store> logger, IProgress progress) {
            _events = events;
            _logger = logger;
            _progress = progress;
            _store = new Dictionary<Network, NetworkStore> {
                { Network.Yammer, new NetworkStore() },
                { Network.LinkedIn, new NetworkStore() },
            };
        }

        public Overview[] GetOverview() {
            return _store.Select(p => new Overview { Network = p.Key, Messages = p.Value.Messages.Count, Users = p.Value.Users.Count})
                .ToArray();
        }

        public Func<Event[], int, Task<int>> FillFromEvents<T>(Dictionary<string, T> dictionary) where T : IExternalEntity
        {
            var count = 0;
            return (events, totalCount) => {
                var eventName = $"{typeof(T).Name}Created";
                var deserialized = events
                    .Where(p => p.EventName == eventName)
                    .Select(p => JsonConvert.DeserializeObject<T>(p.Body))
                    .ToList();
                foreach (var item in deserialized) {
                    count++;
                    _progress.Set(count, totalCount);
                    if (!dictionary.ContainsKey(item.Id))
                        dictionary.Add(item.Id, item);
                }
                return Task.FromResult(events.Last().Id);
            };
        }

        public async Task Hydrate(string userName, string password)
        {
            await Hydrate(userName, password, "Yammer users", Network.Yammer, "UserCreated", _store[Network.Yammer].Users);
            await Hydrate(userName, password, "Yammer messages", Network.Yammer, "MessageCreated", _store[Network.Yammer].Messages);
            await Hydrate(userName, password, "LinkedIn users", Network.LinkedIn, "UserCreated", _store[Network.LinkedIn].Users);

            _logger.LogInformation("Hydrating complete");
            _isHydrated = true;
        }

        private async Task Hydrate<T>(string userName, string password, string entityType, Network network, string eventType, Dictionary<string, T> dictionary)
            where T : IExternalEntity
        {
            var count = await _events.GetCount(userName, network, eventType);
            if (count == 0)
                return;
            _logger.LogInformation($"Hydrating {entityType}...");
            var sw = Stopwatch.StartNew();
            _progress.New();
            Func<Event[], int, Task<int>> eventProcessor = FillFromEvents<T>(dictionary);
            await _events.ReadForward(userName, password, network, count, eventProcessor);
            Log(sw.Elapsed, network, dictionary.Count);
        }

        private void Log(TimeSpan time, Network network, int count)
        {
            _logger.LogInformation($"{count} entities read from network {network} in {time}");
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