using System;

namespace Events
{
    public class User : IExternalEntity
    {
        public string Id { get; set;}
        public string AvatarUrl { get; set; }
        public string Name { get; set;}
        public DateTimeOffset? KnownSince { get; set;}
        public Network Network { get; set;}
        public string Description { get; set; }
        public static User From(Rest.Yammer.User user)
        {
            return new User
            {
                AvatarUrl = user.MugshotUrl.AbsoluteUri,
                Description = user.JobTitle,
                Name = user.Name,
                Network = Network.Yammer,
                Id = $"yammer/user/{user.Id}",
            };
        }
    }
}