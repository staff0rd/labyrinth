using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace YammerScraper
{
    public class RequestLogger
    {
        private readonly string _name;
        private readonly IDictionary<string, RateLimit> _limits;
        private List<RequestLog> _requests;

        public RequestLogger(string name, ReadOnlyDictionary<string, RateLimit> limits)
        {
            _name = $"{name}-requests.json";
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

        public void Log(string category, string endpoint)
        {
            _requests.Add(new RequestLog {Category = category, Endpoint = endpoint, RequestedAt = DateTimeOffset.Now });
            Flush();
        }

        public void Flush()
        {
            Purge();
            var json = JsonConvert.SerializeObject(_requests);
            File.WriteAllText(_name, json);
        }

        public TimeSpan? IsLimited(string category)
        {
            Purge();
            if (_requests.Count(r => r.Category == category) >= _limits[category].RequestCount) {
                var oldestRequestTime =  _requests
                    .Where(r => r.Category == category)
                    .Min(r => r.RequestedAt);
                var wait = DateTimeOffset.Now.Subtract(oldestRequestTime);
                return _limits[category].Duration - wait;
            }
            return null; 
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