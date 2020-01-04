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

        internal static UserCard FromUser(User u)
        {
            return new UserCard
                {
                    AvatarUrl = ScaleMugshot(u),
                    Id = u.Id.ToString(),
                    Network = "Yammer",
                    Name = u.FullName,
                    Description = u.JobTitle,
                };
        }
        private static string ScaleMugshot(User u)
        {
            var result = u.MugshotUrl.ToString();
            if (string.IsNullOrWhiteSpace(result))
                return result;

            return result.Replace("48x48", "128x128");
        }
    }
}
