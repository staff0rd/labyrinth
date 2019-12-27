using System.Collections.Generic;

namespace Robot
{
    public class CurrentUserRequest : Request<string>
    {
        public CurrentUserRequest(IDictionary<string, RateLimit> rates) : base(rates)
        {
        }

        public override string Category => YammerLimits.Other;

        public override string Endpoint => "https://www.yammer.com/api/v1/users/current.json";

        public override string Transform(string response)
        {
            return response;
        }
    }
}