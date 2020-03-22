using System;

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

        public static Message From(Rest.Yammer.Message message)
        {
            return new Message {
                BodyParsed = message.Body.Parsed,
                BodyPlain = message.Body.Plain,
                CreatedAt = DateTime.Parse(message.CreatedAt),
                Id = $"yammer/message/{message.Id}",
                SenderId = $"yammer/user/{message.SenderId}",
                Permalink = message.WebUrl.AbsoluteUri,
            };
        }
    }
}