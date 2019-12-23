using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Serilog;

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
            _requestLog = new RequestLogger(_name, YammerLimits.RateLimits);
        }

        public async Task Automate()
        {
            try
            {
                _logger.Information("Starting automation");
                for (int i = 0; i < 30; i++)
                {
                    await CallEndpoint(YammerLimits.Other, "https://www.yammer.com/api/v1/users/current.json");
                }
                _logger.Information("Automation complete");
            }
            catch (Exception e)
            {
                _logger.Error(e.Message);
            }
            finally {
                _requestLog.Flush();
            }
        }

        private async Task CallEndpoint(string category, string endpoint)
        {
            var delay = _requestLog.IsLimited(category);
            if (delay.HasValue)
            {
                _logger.Warning($"Waiting {delay} for [{category}] {endpoint}");
                await Task.Delay(delay.Value);
            }
            _logger.Information($"Calling {endpoint}");
            _requestLog.Log(category, endpoint);
            try
            {
                var json = await endpoint
                    .WithOAuthBearerToken(_token)
                    .GetStringAsync();
                _logger.Information(json);
            }
            catch (FlurlHttpException ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private async Task GetCurrentUser()
        {
            await Task.Delay(0);
        }
    }
}