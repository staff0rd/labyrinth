using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Robot
{
    public class YammerAutomation {
        private readonly ILogger _logger;
        private readonly EventStoreManager _events;
        private readonly string _token;
        private string _streamName = "Yammer";

        public YammerAutomation(ILogger logger, EventStoreManager events, string token) {
            _logger = logger;
            _events = events;
            _token = token;
        }
        
        public async Task Automate() {
            await _events.CreateOrUpdateProjection($"{_streamName}ApiRequests", Queries.ApiRequests(_streamName));
            await _events.CreateOrUpdateProjection($"{_streamName}Messages", Queries.Messages(_streamName));
            await _events.CreateOrUpdateProjection($"{_streamName}Users", Queries.Users(_streamName));
            await _events.CreateOrUpdateProjection($"{_streamName}Threads", Queries.Threads(_streamName));
            await _events.CreateOrUpdateProjection($"{_streamName}Groups", Queries.Groups(_streamName));

            long? last = null;
                do 
                {
                    var queryString = new { older_than = last };
                    var response = await _events.Get(new MessagesSent(_logger, YammerLimits.RateLimits), queryString, _token);
                    if (response != null) {
                        foreach (var message in response.Messages)
                        {
                            await _events.Sync(message, "Message", await GetMessage(message.Id));
                        }
                        foreach (var user in response.References.Users) {
                            await _events.Sync(user, "User", await GetUser(user.Id), false);
                        }
                        foreach (var message in response.References.Messages) {
                            await _events.Sync(message, "Message", await GetMessage(message.Id), false);
                        }
                        foreach (var group in response.References.Groups) {
                            await _events.Sync(group, "Group", await GetGroup(group.Id), false);
                        }
                        foreach (var thread in response.References.Threads) {
                            await _events.Sync(thread, "Thread", await GetThread(thread.Id), false);
                        }


                        last = response.Messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                    } else {
                        last = null;
                    }
                } while(last != null);
        }

        internal async Task<Thread> GetThread(long id)
        {
            var result = await _events.GetPartitionState($"{_streamName}Threads", id.ToString());

            if (result == "")
                return null;

            return Thread.FromJson(result.ToString());
        }

        internal async Task<Group> GetGroup(long id)
        {
            var result = await _events.GetPartitionState($"{_streamName}Groups", id.ToString());

            if (result == "")
                return null;

            return Group.FromJson(result.ToString());
        }

        public async Task<Message> GetMessage(long id) {
            var result = await _events.GetPartitionState($"{_streamName}Messages", id.ToString());

            if (result == "")
                return null;

            return Message.FromJson(result.ToString());
        }
        
        internal async Task<User> GetUser(long id)
        {
            var result = await _events.GetPartitionState($"{_streamName}Users", id.ToString());

            if (result == "")
                return null;

            return User.FromJson(result.ToString());
        }
    }
}