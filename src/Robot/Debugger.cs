using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Rest.Yammer;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Robot
{
    public class Debugger
    {
        private readonly ILogger _logger;
        public Debugger(ILogger logger) {
            _logger = logger;
        }

        public async Task Go() {
            await Task.Delay(0);
            // var _events = new RestEventManager(_logger);

            // var messages = new Dictionary<long, Message>();
            // await _events.ReadForward("Yammer", (events) => { events
            //     .Where(p => p.Event.EventType == "MessageCreated")
            //     .Select(p => Message.FromJson(p.Event.ToJson()))
            //     .ToList()
            //     .ForEach(message => {
            //         if (!messages.ContainsKey(message.Id))
            //             messages.Add(message.Id, message);
            //     });
            // });
            // var size = messages.Count;
        }
    }
}