using System;

namespace Rest
{
    public class RateLimit
    {
        public TimeSpan Duration { get; private set;}

        public int RequestCount { get; private set; }

        public RateLimit(int requestCount, int durationInSeconds)
        {
            Duration = TimeSpan.FromSeconds(durationInSeconds);
            RequestCount = requestCount;
        }
    }
}