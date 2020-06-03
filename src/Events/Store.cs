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

        public Overview[] GetEntityOverview() {
            return _store.Select(p => new Overview { SourceId = p.Key, 
                Messages = p.Value.Messages.Count,
                Users = p.Value.Users.Count,
                Images = p.Value.Images.Count,
                Topics = p.Value.Topics.Count,
            }).ToArray();
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
                    case (Network.Teams):
                        {
                            await Hydrate(credential, $"{source.Name} users", source.Id, "UserCreated", _store[source.Id].Users);
                            await Hydrate(credential, $"{source.Name} messages", source.Id, "MessageCreated", _store[source.Id].Messages);
                            await Hydrate(credential, $"{source.Name} topics", source.Id, "TopicCreated", _store[source.Id].Topics);
                            await Hydrate(credential, $"{source.Name} images", source.Id, "ImageCreated", _store[source.Id].Images);
                            break;
                        }
                    default: throw new NotImplementedException(source.Network.ToString());
                }
                EnhanceTopics(source);
                EnhanceMessages(source);
                EnhanceImages(source);
            }

            _logger.LogInformation("Hydrating complete");
            _isHydrated = true;
        }

        private void EnhanceTopics(Source source)
        {
            foreach (var message in _store[source.Id].Messages.Values) {
                if (!string.IsNullOrEmpty(message.TopicId)) {
                    if (_store[source.Id].Topics.TryGetValue(message.TopicId, out var topic) &&
                        !topic.Members.ContainsKey(message.SenderId))
                    {
                        topic.Members.Add(message.SenderId, _store[source.Id].Users[message.SenderId]);
                    }
                }
            }
            foreach (var topic in _store[source.Id].Topics.Values)
            {
                if (string.IsNullOrWhiteSpace(topic.Title))
                {
                    topic.Title = string.Join(", ", topic.Members.Values.Take(3).Select(p => p.Name));
                    if(topic.Members.Count > 3)
                        topic.Title += $" +{topic.Members.Count-3}";
                }
            }
        }

        private void EnhanceMessages(Source source)
        {
            foreach ( var message in _store[source.Id].Messages.Values)
            {
                if (!string.IsNullOrEmpty(message.TopicId)) {
                    message.TopicTitle = _store[source.Id].Topics[message.TopicId].Title;
                }
            }
        }
        private void EnhanceImages(Source source)
        {
            foreach ( var image in _store[source.Id].Images.Values)
            {
                if (_store[source.Id].Messages.ContainsKey(image.FromEntityId))
                {
                    var message = _store[source.Id].Messages[image.FromEntityId];
                    image.TopicId = message.TopicId;
                    image.TopicTitle = message.TopicTitle;
                    var user = _store[source.Id].Users[message.SenderId];
                    image.Username = user.Name;
                }
            }
        }

        internal Image[] GetImages(Guid sourceId, string id)
        {
            return _store[sourceId].Images.Select(i => i.Value).Where(p => p.FromEntityId == id).ToArray();
        }

        private async Task Hydrate<T>(Credential credential, string entityType, Guid sourceId, string eventType, Dictionary<string, T> dictionary)
            where T : IExternalEntity
        {
            var count = await _events.GetCount(credential.Username, sourceId, eventType);
            if (count == 0)
                return;
            _logger.LogInformation($"Hydrating {entityType}...");
            var sw = Stopwatch.StartNew();
            await _progress.New();
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

        public void Add(Guid sourceId, Image image)
        {
            _store[sourceId].Images.Add(image.Id.ToString(), image);
            _logger.LogInformation($"Added image {image.Id} to {GetSourceName(sourceId)}");
        }

        public Image GetImage(Guid sourceId, string fromEntityId, string url)
        {
            return _store[sourceId].Images.Select(p => p.Value).FirstOrDefault(p => p.FromEntityId == fromEntityId && p.Url == url); 
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
            _store[sourceId].Topics.TryGetValue(id, out var existing);
            return existing;
        }

        public IEnumerable<Message> GetMessages(Guid sourceId) {
            return _store[sourceId].Messages.ToList().Select(x => x.Value);
        }

        public IEnumerable<Image> GetImages (Guid sourceId) {
            return _store[sourceId].Images.ToList().Select(x => x.Value);
        }

        public IEnumerable<User> GetUsers(Guid sourceId) {
            return _store[sourceId].Users.ToList().Select(x => x.Value);
        }
    }
}