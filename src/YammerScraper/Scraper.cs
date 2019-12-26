using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
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
                    var messages = await Get(new MessagesSent(YammerLimits.RateLimits), queryString);
                    if (messages != null) {
                        foreach (var message in messages) {
                        await _requestLog.Log(_streamName, "MessageCreated", message.ToJson());
                        }
                        last = messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", messages.Count(), last);
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

        private async Task<T> Get<T>(Request<T> request, object queryString)
        {
            var oldest = await _requestLog.Oldest(request.Category);
            if (oldest.HasValue)
            {
                var waitUntil = oldest.Value.Add(request.RateLimit.Duration);
                if (waitUntil > DateTimeOffset.Now) {
                    var delay = waitUntil.Subtract(DateTimeOffset.Now);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, request.Category, request.Endpoint);
                    await Task.Delay(delay);
                }
            }
            await _requestLog.Log(_streamName, "ApiRequest", new RequestLog {Category = request.Category, Endpoint = request.Endpoint, RequestedAt = DateTimeOffset.Now, RateLimit = request.RateLimit}.ToJson());
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