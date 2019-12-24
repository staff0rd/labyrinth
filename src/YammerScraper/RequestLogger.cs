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
        private readonly string _name;
        private readonly IEventStoreConnection _events;
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Projections _projections;
        private List<RequestLog> _requests;

        public RequestLogger(Microsoft.Extensions.Logging.ILogger logger, string name, ReadOnlyDictionary<string, RateLimit> limits)
        {
            _name = $"{name}Requests";
            _events = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), _name);
            _events.ConnectAsync().Wait();
            _projections = new Projections(logger, new UserCredentials("admin", "changeit"));
            _projections.CreateOrUpdate(_name, Queries.ApiRequests(_name)).Wait();
            _limits = limits;
            _requests = ReadRequests();
            Purge();
        }

        private List<RequestLog> ReadRequests()
        {
            if (File.Exists(_name))
            {
                var json = File.ReadAllText(_name);
                return JsonConvert.DeserializeObject<List<RequestLog>>(json);
            }
            return new List<RequestLog>();
        }

        public async Task Log(string category, string endpoint, RateLimit rate)
        {
            var ev = new RequestLog {Category = category, Endpoint = endpoint, RequestedAt = DateTimeOffset.Now, RateLimit = rate}.ToEvent("ApiRequest");

            await _events.AppendToStreamAsync(_name, ExpectedVersion.Any, ev);
        }

        public async Task<DateTimeOffset?> Oldest(string category)
        {
            var result = await _projections.GetState(_name, category);

            if (result == "")
                return null;

            dynamic json = JValue.Parse(result);

            DateTimeOffset? oldest = json.oldest;

            return oldest;
        }

        private void Purge()
        {
            var limited = new List<RequestLog>();
            _requests.ForEach(request =>
            {
                var duration = _limits[request.Category].Duration;
                if (request.RequestedAt >= DateTimeOffset.Now.Add(-duration))
                    limited.Add(request);
            });
            _requests = limited;
        }
    }
}