using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YammerScraper
{
    public class MessagesSent : Request<MessagesSentResponse>
    {
        private readonly ILogger _logger;

        public MessagesSent(ILogger logger, IDictionary<string, RateLimit> rates) : base(rates)
        {
            _logger = logger;
        }

        public override string Category => YammerLimits.Messages;

        public override string Endpoint => "https://www.yammer.com/api/v1/messages/sent.json";

        public override MessagesSentResponse Transform(string response)
        {
            dynamic json = JToken.Parse(response);
            
            JArray messages = json.messages;
            JArray references = json.references;

            var result  = new MessagesSentResponse();
            foreach ( var message in messages) {
                result.Messages.Add(Message.FromJson(message.ToString()));
            }

            foreach ( var reference in references)
            {
                var type = reference.SelectToken("type").ToString();

                switch (type) {
                    case "message": result.References.Messages.Add(Message.FromJson(reference.ToString()));
                    break;
                    case "user": result.References.Users.Add(User.FromJson(reference.ToString()));
                    break;
                    case "thread": result.References.Threads.Add(Thread.FromJson(reference.ToString()));
                    break;
                    case "group": result.References.Groups.Add(Group.FromJson(reference.ToString()));
                    break;
                    default: _logger.LogWarning("Unknown type: {type}", type);
                    break;
                }
            }

            return result;
        }
    }
}