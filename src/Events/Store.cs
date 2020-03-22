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

        public List<Source> Sources { get; private set; } = new List<Source>();

        public Dictionary<Guid, NetworkStore> _store;

        public Store(EventRepository events, ILogger<Store> logger, IProgress progress) {
            _events = events;
            _logger = logger;
            _progress = progress;
            _store = new Dictionary<Guid, NetworkStore>();
        }

        public void UpdateSources(IEnumerable<Source> sources)
        {
            Sources = sources.ToList();
            foreach (var source in Sources)
            {
                if (!_store.ContainsKey(source.Id))
                    _store.Add(source.Id, new NetworkStore());
            }
        }

        public void AddSource(Source source)
        {
            Sources.Add(source);
            _store.Add(source.Id, new NetworkStore());
        }

        internal object GetUsers(object sourceId)
        {
            throw new NotImplementedException();
        }

        public Overview[] GetOverview() {
            return _store.Select(p => new Overview { SourceId = p.Key, Messages = p.Value.Messages.Count, Users = p.Value.Users.Count})
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

        public async Task Hydrate(Credential credential)
        {
            foreach (var source in Sources)
            {
                switch(source.Network)
                {
                    case (Network.Yammer):
                    {
                        await Hydrate(credential, $"{source.Name} users", source.Id, "UserCreated", _store[source.Id].Users);
                        await Hydrate(credential, $"{source.Name} messages", source.Id, "MessageCreated", _store[source.Id].Messages);
                        break;
                    }
                    case (Network.LinkedIn): {
                        await Hydrate(credential, $"{source.Name} users", source.Id, "UserCreated", _store[source.Id].Users);
                        break;
                    }
                    case (Network.Teams): {
                        break;
                    }
                    default: throw new NotImplementedException(source.Network.ToString());
                }
            }

            _logger.LogInformation("Hydrating complete");
            _isHydrated = true;
        }

        private async Task Hydrate<T>(Credential credential, string entityType, Guid sourceId, string eventType, Dictionary<string, T> dictionary)
            where T : IExternalEntity
        {
            var count = await _events.GetCount(credential.Username, sourceId, eventType);
            if (count == 0)
                return;
            _logger.LogInformation($"Hydrating {entityType}...");
            var sw = Stopwatch.StartNew();
            _progress.New();
            Func<Event[], int, Task<int>> eventProcessor = FillFromEvents<T>(dictionary);
            await _events.ReadForward(credential, sourceId, count, eventProcessor);
            Log(sw.Elapsed, sourceId, dictionary.Count);
        }

        private void Log(TimeSpan time, Guid sourceId, int count)
        {
            _logger.LogInformation($"{count} entities read from {GetSourceName(sourceId)} in {time}");
        }

        private string GetSourceName(Guid sourceId)
        {
            return Sources.Single(p => p.Id == sourceId).Name;
        }

        public void Add(Guid sourceId, User user)
        {
            _store[sourceId].Users.Add(user.Id, user);
            _logger.LogInformation($"Added user {user.Id} to {GetSourceName(sourceId)}");
        }

        public void Add(Guid sourceId, Message message)
        {
            _store[sourceId].Messages.Add(message.Id, message);
            _logger.LogInformation($"Added message {message.Id} to {GetSourceName(sourceId)}");
        }

        public void Add(Guid sourceId, Topic topic)
        {
            _store[sourceId].Topics.Add(topic.Id, topic);
            _logger.LogInformation($"Added topic {topic.Id} to {GetSourceName(sourceId)}");
        }

        public User GetUser(Guid sourceId, string id)
        {
            _store[sourceId].Users.TryGetValue(id, out var existing);
            return existing;
        }

        public Message GetMessage(Guid sourceId, string id)
        {
            _store[sourceId].Messages.TryGetValue(id, out var existing);
            return existing;
        }

        internal Topic GetTopic(Guid sourceId, string id)
        {
            throw new NotImplementedException();
        }

        internal Topic GetTopics(Guid sourceId, string id)
        {
            _store[sourceId].Topics.TryGetValue(id, out var existing);
            return existing;
        }

        public IEnumerable<Message> GetMessages(Guid sourceId) {
            return _store[sourceId].Messages.ToList().Select(x => x.Value);
        }
        public IEnumerable<User> GetUsers(Guid sourceId) {
            return _store[sourceId].Users.ToList().Select(x => x.Value);
        }
    }
}