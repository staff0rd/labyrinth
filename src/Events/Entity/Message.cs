using System;

namespace Events
{
    public class Message : IExternalEntity
    {
        public Guid Id { get; set; }
        public Guid SenderId { get; set;}
        public string BodyPlain { get; set;}
        public string BodyParsed { get; set; }
        public DateTime CreatedAt { get; set; }
        public Network Network { get; set;}

        public static Message From(Rest.Yammer.Message message)
        {
            return new Message {
                BodyParsed = message.Body.Parsed,
                BodyPlain = message.Body.Plain,
                CreatedAt = DateTime.Parse(message.CreatedAt),
                Id = $"yammer/message/{message.Id}",
                Network = Network.Yammer,
                SenderId = $"yammer/user/{message.SenderId}",
            };
        }
    }
}