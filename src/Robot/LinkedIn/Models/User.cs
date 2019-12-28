using System;
using Newtonsoft.Json;

namespace Robot.LinkedIn
{
    public class User {
        public string Id => ProfileUrl;
        public string Name { get; set; }

        public string MugshotUrl { get; set; }

        public string ProfileUrl { get; set; }

        public string Occupation { get; set; }

        public DateTimeOffset Connected { get; set; }

        internal static User From(ConnectionCard card)
        {
            return new User {
                Name = card.Name,
                MugshotUrl = card.MugshotUrl,
                ProfileUrl = card.ProfileUrl,
                Occupation = card.Occupation,
                Connected = card.ConnectedDate
            };
        }

        public static User FromJson(string json) => JsonConvert.DeserializeObject<User>(json);
    }
}