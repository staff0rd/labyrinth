using System.Collections.Generic;

namespace Rest
{
    public abstract class Request<T> {
        private readonly IDictionary<string, RateLimit> _rates;

        public Request() { }

        public Request(IDictionary<string, RateLimit> rates) {
            _rates = rates;
        }

        public abstract string Category { get; }

        public RateLimit RateLimit => _rates.ContainsKey(Category) ? _rates[Category] : new RateLimit(99999, 1);

        public abstract string Endpoint { get; }

        public abstract T Transform(string response);
    }
}