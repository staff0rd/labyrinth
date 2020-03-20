using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rest.Teams
{
    public class GetChatsRequest : Request<GetChatsResponse>
    {
        private readonly ILogger _logger;

        public GetChatsRequest(ILogger logger, IDictionary<string, RateLimit> rates) : base(rates)
        {
            _logger = logger;
        }

        public override string Category => TeamsLimits.All;

        public override string Endpoint => "https://www.yammer.com/api/v1/messages/sent.json";

        public override GetChatsResponse Transform(string response)
        {
            return GetChatsResponse.FromJson(response);
        }
    }
}