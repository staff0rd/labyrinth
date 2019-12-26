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

namespace YammerScraper
{
    public class RequestLogger
    {
        private readonly string _streamName;
        private readonly IEventStoreConnection _store;
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Projections _projections;

        public RequestLogger(Microsoft.Extensions.Logging.ILogger logger, string streamName, ReadOnlyDictionary<string, RateLimit> limits)
        {
            _streamName = streamName;
            _store = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "YammerScraper");
            _store.ConnectAsync().Wait();
            _projections = new Projections(logger, new UserCredentials("admin", "changeit"));
            _projections.CreateOrUpdate(_streamName, Queries.ApiRequests(_streamName)).Wait();
            _limits = limits;
        }

        public async Task Log(string streamName, string eventType, string payload)
        {
            var ev = payload.ToEvent(eventType);

            await _store.AppendToStreamAsync(_streamName, ExpectedVersion.Any, ev);
        }

        public async Task<DateTimeOffset?> Oldest(string category)
        {
            var result = await _projections.GetPartitionState(_streamName, category);

            if (result == "")
                return null;

            dynamic json = JValue.Parse(result);

            DateTimeOffset? oldest = json.oldest;

            return oldest;
        }
    }
}