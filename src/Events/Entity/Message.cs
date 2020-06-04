using System;
using Microsoft.Graph;

namespace Events
{
    public class Message : IExternalEntity
    {
        public string Id { get; set; }
        public string SenderId { get; set;}
        public string TopicId { get; set; }
        public string BodyPlain { get; set;}
        public string BodyParsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SourceId { get; set;}
        public string Permalink { get; set; }
        public Network Network { get; set; }
        public string TopicTitle { get; internal set; }

        public static Message From (ChatMessage message, Guid sourceId, string topicId)
        {
            return new Message {
                BodyParsed = message.Body.Content,
                BodyPlain = message.Body.Content,
                CreatedAt = message.CreatedDateTime?.DateTime ?? DateTimeOffset.UtcNow.DateTime,
                Id = message.Id,
                SenderId = message.From.User?.Id ?? User.UnknownUserId,
                TopicId = topicId,
                SourceId = sourceId,
                Network = Network.Teams,
            };
        }
        public static Message From(Rest.Yammer.Message message, Guid sourceId)
        {
            return new Message {
                BodyParsed = message.Body.Parsed,
                BodyPlain = message.Body.Plain,
                CreatedAt = DateTime.Parse(message.CreatedAt),
                Id = $"yammer/message/{message.Id}",
                SenderId = $"yammer/user/{message.SenderId}",
                Permalink = message.WebUrl.AbsoluteUri,
                SourceId = sourceId,
                Network = Network.Yammer,
            };
        }
    }
}