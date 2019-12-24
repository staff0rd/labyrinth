using System;
using System.Collections.Generic;
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

        private string _name => "Yammer";

        public Scraper(Uri url, string token, ILogger logger)
        {
            _url = url;
            _token = token;
            _logger = logger;
            _requestLog = new RequestLogger(logger, _name, YammerLimits.RateLimits);
        }

        public async Task Automate()
        {
            try
            {
                _logger.LogInformation("Starting automation");
                for (int i = 0; i < 50; i++)
                {
                    await CallEndpoint(YammerLimits.Other, "https://www.yammer.com/api/v1/users/current.json", YammerLimits.RateLimits[YammerLimits.Other]);
                }
                _logger.LogInformation("Automation complete");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        private async Task CallEndpoint(string category, string endpoint, RateLimit rate)
        {
            var oldest = await _requestLog.Oldest(category);
            if (oldest.HasValue)
            {
                var waitUntil = oldest.Value.Add(rate.Duration);
                if (waitUntil > DateTimeOffset.Now) {
                    var delay = waitUntil.Subtract(DateTimeOffset.Now);
                    _logger.LogWarning("Waiting {delay} for [{category}] {endpoint}", delay, category, endpoint);
                    await Task.Delay(delay);
                }
            }
            _logger.LogWarning("Calling {endpoint}", endpoint);
            await _requestLog.Log(category, endpoint, rate);
            try
            {
                // var json = await endpoint
                //     .WithOAuthBearerToken(_token)
                //     .GetStringAsync();
                // _logger.Information(json);
            }
            catch (FlurlHttpException ex)
            {
                _logger.LogError(default(EventId), ex, ex.Message);
            }
        }

        private async Task GetCurrentUser()
        {
            await Task.Delay(0);
        }
    }
}