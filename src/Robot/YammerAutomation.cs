using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.Yammer;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Robot
{
    public class YammerAutomation {
        private readonly ILogger _logger;
        private readonly EventStoreManager _events;
        private readonly string _token;
        private string _streamName = StreamNames.Yammer;

        public YammerAutomation(ILogger logger, string token) {
            _logger = logger;
            _events = new EventStoreManager(logger, YammerLimits.RateLimits);
            _token = token;
        }
        
        public async Task Automate() {
            await _events.MigrateProjections();
            
            long? last = null;
                do 
                {
                    var queryString = new { older_than = last };
                    var response = await _events.Get(_streamName, new MessagesSentRequest(_logger, YammerLimits.RateLimits), queryString, _token);
                    if (response != null) {
                        foreach (var message in response.Messages)
                        {
                            var existing = await new GetMessageById().Get(_events, message.Id.ToString());
                            await _events.Sync(_streamName, message, existing);
                        }
                        foreach (var user in response.References.Users) {
                            var existing = await new GetUserById().Get(_events, user.Id.ToString());
                            await _events.Sync(_streamName, user, existing, false);
                        }
                        foreach (var message in response.References.Messages) {
                            var existing = await new GetMessageById().Get(_events, message.Id.ToString());
                            await _events.Sync(_streamName, message, existing, false);
                        }
                        foreach (var group in response.References.Groups) {
                            var existing = await new GetGroupById().Get(_events, group.Id.ToString());
                            await _events.Sync(_streamName, group, existing, false);
                        }
                        foreach (var thread in response.References.Threads) {
                            var existing = await new GetThreadById().Get(_events, thread.Id.ToString());
                            await _events.Sync(_streamName, thread, existing, false);
                        }


                        last = response.Messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                    } else {
                        last = null;
                    }
                } while(last != null);
        }
    }
}