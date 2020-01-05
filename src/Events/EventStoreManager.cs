using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.SystemData;
using Microsoft.Extensions.Logging;
using Flurl.Http;
using JsonDiffPatchDotNet;
using Rest;

namespace Events
{
    public class EventStoreManager
    {
        private readonly IEventStoreConnection _store;
        private readonly IDictionary<string, RateLimit> _limits;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public async Task MigrateProjections()
        {
            await new GetApiRequestsByCategory().CreateOrUpdate(this);
            
            await new Yammer.GetMessageById().CreateOrUpdate(this);
            await new Yammer.GetUserById().CreateOrUpdate(this);
            await new Yammer.GetThreadById().CreateOrUpdate(this);
            await new Yammer.GetGroupById().CreateOrUpdate(this);
            await new Yammer.GetUsers().CreateOrUpdate(this);
            await new Yammer.GetMessages().CreateOrUpdate(this);
            await new Yammer.GetOverview().CreateOrUpdate(this);

            await new LinkedIn.GetUserById().CreateOrUpdate(this);
            await new LinkedIn.GetUsers().CreateOrUpdate(this);
        }

        private readonly Projections _projections;

        public EventStoreManager(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
            
            _store = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"), "Robot");
            _store.ConnectAsync().Wait();

            _projections = new Projections(logger, new UserCredentials("admin", "changeit"));
        }

        public EventStoreManager(Microsoft.Extensions.Logging.ILogger logger, ReadOnlyDictionary<string, RateLimit> limits) : this( logger)
        {
            _limits = limits;
        }

        public Task CreateOrUpdateProjection(string projectionName, string query) {
            return _projections.CreateOrUpdate(projectionName, query);
        }

        public Task<string> GetProjection(string projectionName) {
            return _projections.GetProjection(projectionName);
        }

        internal Task<string> GetProjection(string projectionName, string partition) {
            return _projections.GetProjection(projectionName, partition);
        }

        public async Task<T> Get<T>(string streamName, Request<T> request, object queryString, string token)
        {
            var oldest = await new GetApiRequestsByCategory().GetOldest(this, request.Category);
            if (oldest.HasValue)
            {
                var waitUntil = oldest.Value.Add(request.RateLimit.Duration);
                if (waitUntil > DateTimeOffset.Now) {
                    var delay = waitUntil.Subtract(DateTimeOffset.Now);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, request.Category, request.Endpoint);
                    await Task.Delay(delay);
                }
            }
            await RaiseEvent(streamName, "ApiRequest", new RequestLog {Category = request.Category, Endpoint = request.Endpoint, RequestedAt = DateTimeOffset.Now, RateLimit = request.RateLimit}.ToJson());
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

            await _store.AppendToStreamAsync(streamName, ExpectedVersion.Any, ev);

            _logger.LogInformation("Raised {EventType} in {StreamName}", eventType, streamName);
        }

        private Task<DateTimeOffset?> GetOldestApiRequest(string category)
        {
            return new GetApiRequestsByCategory().GetOldest(this, category);
        }

        public async Task Sync<T>(string streamName, T payload, T existing, bool update = true) {
            if (existing != null)
            {
                if (update) {
                    var jdp = new JsonDiffPatch();
                    var output = jdp.Diff(existing.ToJson(), payload.ToJson());
                    if (output != null)
                    {
                        await RaiseEvent(streamName, $"{payload.GetType().Name}Updated", payload.ToJson());
                    }
                }
            }
            else
            {
                await RaiseEvent(streamName, $"{payload.GetType().Name}Created", payload.ToJson());
            }
        }
    }
}