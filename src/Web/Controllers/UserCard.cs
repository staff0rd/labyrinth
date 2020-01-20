using System;
using Rest.Yammer;

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

        internal static UserCard FromUser(Events.User u)
        {
            return new UserCard
                {
                    AvatarUrl = ScaleMugshot(u.AvatarUrl),
                    Id = u.Id,
                    Network = "Yammer",
                    Name = u.Name,
                    Description = u.Description,
                };
        }
        private static string ScaleMugshot(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return url;

            return url.Replace("48x48", "128x128");
        }
    }
}
