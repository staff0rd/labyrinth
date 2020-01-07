using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Rest.Yammer;

namespace Events
{
    public class YammerStore
    {
        private ReadOnlyDictionary<long, Message> _messages;
        public ReadOnlyDictionary<long, Message> Messages => _messages;
        private readonly EventStoreManager _events;

        public string StreamName => "Yammer";

        public YammerStore(EventStoreManager store) {
            _events = store;
        }

        public async Task Hydrate()
        {
            var messages = new Dictionary<long, Message>();
            await _events.ReadForward(StreamName, (events) => { events
                .Where(p => p.Event.EventType == "MessageCreated")
                .Select(p => Message.FromJson(p.Event.ToJson()))
                .ToList()
                .ForEach(message => {
                    if (!messages.ContainsKey(message.Id))
                        messages.Add(message.Id, message);
                });
            });
            _messages = new ReadOnlyDictionary<long, Message>(messages);
        }
    }
}