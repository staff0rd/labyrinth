using System;

namespace Rest
{
    public class RequestLog
    {
        public string Endpoint { get; set;}
        
        public DateTimeOffset RequestedAt { get; set; }

        public string Category { get; set; }
        
        public RateLimit RateLimit { get; set;}
    }
}