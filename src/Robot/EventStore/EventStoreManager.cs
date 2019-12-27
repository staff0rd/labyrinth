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
using Flurl.Http;
using JsonDiffPatchDotNet;

namespace Robot
{
    public class EventStoreManager
    {
        private readonly string _streamName;
        private readonly IEventStoreConnection _store;
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly Projections _projections;

        public EventStoreManager(Microsoft.Extensions.Logging.ILogger logger, string streamName)
        {
            _streamName = streamName;
            _logger = logger;
            
            _store = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "Robot");
            _store.ConnectAsync().Wait();

            _projections = new Projections(logger, new UserCredentials("admin", "changeit"));
        }

        public EventStoreManager(Microsoft.Extensions.Logging.ILogger logger, string streamName, ReadOnlyDictionary<string, RateLimit> limits) : this( logger, streamName)
        {
            _limits = limits;
        }

        public Task CreateOrUpdateProjection(string projectionName, string query) {
            return _projections.CreateOrUpdate(projectionName, query);
        }

        public Task<string> GetPartitionState(string projectionName, string partition) {
            return _projections.GetPartitionState(projectionName, partition);
        }

        public async Task<T> Get<T>(Request<T> request, object queryString, string token)
        {
            var oldest = await GetOldestApiRequest(request.Category);
            if (oldest.HasValue)
            {
                var waitUntil = oldest.Value.Add(request.RateLimit.Duration);
                if (waitUntil > DateTimeOffset.Now) {
                    var delay = waitUntil.Subtract(DateTimeOffset.Now);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, request.Category, request.Endpoint);
                    await Task.Delay(delay);
                }
            }
            await RaiseEvent(_streamName, "ApiRequest", new RequestLog {Category = request.Category, Endpoint = request.Endpoint, RequestedAt = DateTimeOffset.Now, RateLimit = request.RateLimit}.ToJson());
            try
            {
                var url = request.Endpoint
                    .WithOAuthBearerToken(token);
                
                if (queryString != null) {
                    url.SetQueryParams(queryString);
                }
                    
                var json = await url.GetStringAsync();
                return request.Transform(json);
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(default(EventId), ex, ex.Message);
                return default(T);
            }
        }

        private async Task RaiseEvent(string streamName, string eventType, string payload)
        {
            var ev = payload.ToEvent(eventType);

            await _store.AppendToStreamAsync(_streamName, ExpectedVersion.Any, ev);

            _logger.LogInformation("Raised {EventType} in {StreamName}", eventType, _streamName);
        }

        private async Task<DateTimeOffset?> GetOldestApiRequest(string category)
        {
            var result = await _projections.GetPartitionState($"{_streamName}ApiRequests", category);

            if (result == "")
                return null;

            dynamic json = JValue.Parse(result);

            DateTimeOffset? oldest = json.oldest;

            return oldest;
        }

        public async Task Sync<T>(T payload, string type, T existing, bool update = true) {
            if (existing != null)
            {
                if (update) {
                    var jdp = new JsonDiffPatch();
                    var output = jdp.Diff(existing.ToJson(), payload.ToJson());
                    if (output != null)
                    {
                        await RaiseEvent(_streamName, $"{type}Updated", payload.ToJson());
                    }
                }
            }
            else
            {
                await RaiseEvent(_streamName, $"{type}Created", payload.ToJson());
            }
        }
    }
}