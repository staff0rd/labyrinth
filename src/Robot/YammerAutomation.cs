using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;
using Events;
using Events.Yammer;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rest.Yammer;

namespace Robot
{
    public class YammerAutomation {
        private readonly ILogger _logger;
        private readonly EventRepository _events;
        private readonly RestEventManager _rest;
        private readonly Store _store;
        private readonly string _token;

        public YammerAutomation(ILogger logger, EventRepository events)
            : this(logger, events, null) {}

        public YammerAutomation(ILogger logger, EventRepository events, string token) {
            _logger = logger;
            _events = events;
            _rest = new RestEventManager(logger, events);
            _store = new Store(_events, _logger);
            _token = token;
        }
        
        
        
    }
}