using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YammerScraper
{
    public class MessagesSent : Request<IEnumerable<Message>>
    {
        public MessagesSent(IDictionary<string, RateLimit> rates) : base(rates)
        {
        }

        public override string Category => YammerLimits.Messages;

        public override string Endpoint => "https://www.yammer.com/api/v1/messages/sent.json";

        public override IEnumerable<Message> Transform(string response)
        {
            dynamic json = JToken.Parse(response);
            JArray messages = json.messages;

            foreach ( var message in messages) {
                yield return Message.FromJson(message.ToString());
            }
        }
    }
}