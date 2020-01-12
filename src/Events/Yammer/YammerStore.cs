using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Rest.Yammer;

namespace Events
{
    public class YammerStore
    {
        private ReadOnlyDictionary<long, Message> _messages;
        public ReadOnlyDictionary<long, Message> Messages => _messages;
        private readonly EventRepository _events;

        public string StreamName => "Yammer";

        public YammerStore(EventRepository events) {
            _events = events;
        }

        public async Task Hydrate()
        {
            var messages = new Dictionary<long, Message>();
            await _events.ReadForward(Network.Yammer, (events) => { events
                .Where(p => p.EventName == "MessageCreated")
                .Select(p => Message.FromJson(p.Body))
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