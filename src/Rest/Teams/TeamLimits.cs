using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rest.Teams
{
    public class TeamsLimits
    {
        public const string All = "All";

        public static ReadOnlyDictionary<string, RateLimit> RateLimits => new ReadOnlyDictionary<string, RateLimit>(
            new Dictionary<string, RateLimit> {
                { All, new RateLimit(99999, 1)},
            });
    }
}