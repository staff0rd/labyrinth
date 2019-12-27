using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using JsonDiffPatchDotNet;
using Microsoft.Extensions.Logging;

namespace Robot
{
    public class Scraper
    {
        private readonly Uri _url;

        public string _token { get; }

        private readonly ILogger _logger;

        private EventStoreManager _events;

        private string _streamName => "Yammer";

        public Scraper(Uri url, string token, ILogger logger)
        {
            _url = url;
            _token = token;
            _logger = logger;
            _events = new EventStoreManager(logger, _streamName, YammerLimits.RateLimits);
        }

        public async Task Automate()
        {
            try
            {
                _logger.LogInformation("Starting automation");

                var yammer = new YammerAutomation(_logger, _events, _token);
                await yammer.Automate();
                
                _logger.LogInformation("Automation complete");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }

        
    }
}