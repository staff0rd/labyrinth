using System;

namespace Web.Controllers
{
    public class UserCard
    {
        public string Id { get; set;}
        public string AvatarUrl { get; set; }
        public string Name { get; set;}
        public DateTimeOffset? KnownSince { get; set;}
        public string Network { get; set;}
        public string Description { get; internal set; }
    }
}
