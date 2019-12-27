using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Projections;
using EventStore.ClientAPI.SystemData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace Robot
{
    public class RequestLogger
    {
        private readonly string _streamName;
        private readonly IEventStoreConnection _store;
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly Projections _projections;

        public RequestLogger(Microsoft.Extensions.Logging.ILogger logger, string streamName, ReadOnlyDictionary<string, RateLimit> limits)
        {
            _streamName = streamName;
            _limits = limits;
            _logger = logger;
            
            _store = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "Robot");
            _store.ConnectAsync().Wait();

            _projections = new Projections(logger, new UserCredentials("admin", "changeit"));
            _projections.CreateOrUpdate($"{_streamName}ApiRequests", Queries.ApiRequests(_streamName)).Wait();
            _projections.CreateOrUpdate($"{_streamName}Messages", Queries.Messages(_streamName)).Wait();
            _projections.CreateOrUpdate($"{_streamName}Users", Queries.Users(_streamName)).Wait();
            _projections.CreateOrUpdate($"{_streamName}Threads", Queries.Threads(_streamName)).Wait();
            _projections.CreateOrUpdate($"{_streamName}Groups", Queries.Groups(_streamName)).Wait();
        }

        public async Task RaiseEvent(string streamName, string eventType, string payload)
        {
            var ev = payload.ToEvent(eventType);

            await _store.AppendToStreamAsync(_streamName, ExpectedVersion.Any, ev);

            _logger.LogInformation("Raised {EventType} in {StreamName}", eventType, _streamName);
        }

        public async Task<DateTimeOffset?> GetOldestApiRequest(string category)
        {
            var result = await _projections.GetPartitionState($"{_streamName}ApiRequests", category);

            if (result == "")
                return null;

            dynamic json = JValue.Parse(result);

            DateTimeOffset? oldest = json.oldest;

            return oldest;
        }

        internal async Task<Thread> GetThread(long id)
        {
            var result = await _projections.GetPartitionState($"{_streamName}Threads", id.ToString());

            if (result == "")
                return null;

            return Thread.FromJson(result.ToString());
        }

        internal async Task<Group> GetGroup(long id)
        {
            var result = await _projections.GetPartitionState($"{_streamName}Groups", id.ToString());

            if (result == "")
                return null;

            return Group.FromJson(result.ToString());
        }

        public async Task<Message> GetMessage(long id) {
            var result = await _projections.GetPartitionState($"{_streamName}Messages", id.ToString());

            if (result == "")
                return null;

            return Message.FromJson(result.ToString());
        }
        
        internal async Task<User> GetUser(long id)
        {
            var result = await _projections.GetPartitionState($"{_streamName}Users", id.ToString());

            if (result == "")
                return null;

            return User.FromJson(result.ToString());
        }
    }
}