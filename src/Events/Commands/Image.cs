using System;

namespace Events
{
    public class Image : IExternalEntity
    {
        public string Url { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public string FromEntityId { get; set; }

        public string Id { get; set; }

        public Network Network { get; set; }
        public DateTimeOffset Created { get; set; }
        public string TopicId { get; internal set; }
        public string TopicTitle { get; internal set; }
        public string Username { get; internal set; }
    }
}