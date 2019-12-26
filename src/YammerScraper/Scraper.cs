using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using JsonDiffPatchDotNet;
using Microsoft.Extensions.Logging;

namespace YammerScraper
{
    public class Scraper
    {
        private readonly Uri _url;

        public string _token { get; }

        private readonly ILogger _logger;

        private RequestLogger _requestLog;

        private string _streamName => "Yammer";

        public Scraper(Uri url, string token, ILogger logger)
        {
            _url = url;
            _token = token;
            _logger = logger;
            _requestLog = new RequestLogger(logger, _streamName, YammerLimits.RateLimits);
        }

        public async Task Automate()
        {
            try
            {
                _logger.LogInformation("Starting automation");

                long? last = null;
                do 
                {
                    var queryString = new { older_than = last };
                    var response = await Get(new MessagesSent(_logger, YammerLimits.RateLimits), queryString);
                    if (response != null) {
                        foreach (var message in response.Messages)
                        {
                            await Sync(message, "Message", await _requestLog.GetMessage(message.Id));
                        }
                        foreach (var user in response.References.Users) {
                            await Sync(user, "User", await _requestLog.GetUser(user.Id), false);
                        }
                        foreach (var message in response.References.Messages) {
                            await Sync(message, "Message", await _requestLog.GetMessage(message.Id), false);
                        }
                        foreach (var group in response.References.Groups) {
                            await Sync(group, "Group", await _requestLog.GetGroup(group.Id), false);
                        }
                        foreach (var thread in response.References.Threads) {
                            await Sync(thread, "Thread", await _requestLog.GetThread(thread.Id), false);
                        }


                        last = response.Messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                    } else {
                        last = null;
                    }
                } while(last != null);
                
                _logger.LogInformation("Automation complete");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        public async Task Sync<T>(T payload, string type, T existing, bool update = true) {
            if (existing != null)
            {
                if (update) {
                    var jdp = new JsonDiffPatch();
                    var output = jdp.Diff(existing.ToJson(), payload.ToJson());
                    if (output != null)
                    {
                        await _requestLog.RaiseEvent(_streamName, $"{type}Updated", payload.ToJson());
                    }
                }
            }
            else
            {
                await _requestLog.RaiseEvent(_streamName, $"{type}Created", payload.ToJson());
            }
        }

        private async Task<T> Get<T>(Request<T> request, object queryString)
        {
            var oldest = await _requestLog.GetOldestApiRequest(request.Category);
            if (oldest.HasValue)
            {
                var waitUntil = oldest.Value.Add(request.RateLimit.Duration);
                if (waitUntil > DateTimeOffset.Now) {
                    var delay = waitUntil.Subtract(DateTimeOffset.Now);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, request.Category, request.Endpoint);
                    await Task.Delay(delay);
                }
            }
            await _requestLog.RaiseEvent(_streamName, "ApiRequest", new RequestLog {Category = request.Category, Endpoint = request.Endpoint, RequestedAt = DateTimeOffset.Now, RateLimit = request.RateLimit}.ToJson());
            try
            {
                var url = request.Endpoint
                    .WithOAuthBearerToken(_token);
                
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
    }
}