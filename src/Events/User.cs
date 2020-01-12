using System;

namespace Events
{
    public class User : IExternalEntity
    {
        public Guid Id { get; set;}
        public string AvatarUrl { get; set; }
        public string Name { get; set;}
        public DateTimeOffset? KnownSince { get; set;}
        public Network Network { get; set;}
        public string Description { get; internal set; }
        public string ExternalId { get; set; }
    }
}