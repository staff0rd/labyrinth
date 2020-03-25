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
    }
}