using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rest.Yammer
{
    public class YammerLimits
    {
        public const string Autocomplete = "Autocomplete";

        public const string Messages = "Messages";

        public const string Notifications = "Notifications";

        public const string Other = "Other";

        public static ReadOnlyDictionary<string, RateLimit> RateLimits => new ReadOnlyDictionary<string, RateLimit>(
            new Dictionary<string, RateLimit> {
                { Autocomplete, new RateLimit(10, 10)},
                { Messages, new RateLimit(10, 30)},
                { Notifications, new RateLimit(10, 30)},
                { Other, new RateLimit(10, 10)},
            });
    }
}