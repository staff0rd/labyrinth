using System.Collections.Generic;

namespace YammerScraper
{
    public abstract class Request<T> {
        private readonly IDictionary<string, RateLimit> _rates;

        public Request(IDictionary<string, RateLimit> rates) {
            _rates = rates;
        }

        public abstract string Category { get; }

        public RateLimit RateLimit => _rates[Category];

        public abstract string Endpoint { get; }

        public abstract T Transform(string response);
    }
}