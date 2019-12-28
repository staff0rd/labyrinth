using System;
using Newtonsoft.Json;

namespace Events.LinkedIn
{
    public class User {
        public string Id => ProfileUrl;
        public string Name { get; set; }

        public string MugshotUrl { get; set; }

        public string ProfileUrl { get; set; }

        public string Occupation { get; set; }

        public DateTimeOffset Connected { get; set; }

        public static User Create(string name, string mugshotUrl, string profileUrl, string occupation, DateTimeOffset connected)
        {
            return new User {
                Name = name,
                MugshotUrl = mugshotUrl,
                ProfileUrl = profileUrl,
                Occupation = occupation,
                Connected = connected
            };
        }

        public static User FromJson(string json) => JsonConvert.DeserializeObject<User>(json);
    }
}