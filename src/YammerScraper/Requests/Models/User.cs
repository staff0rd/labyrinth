// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using YammerScraper;
//
//    var user = User.FromJson(jsonString);

namespace YammerScraper
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class User
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("job_title")]
        public string JobTitle { get; set; }

        [JsonProperty("network_id")]
        public long NetworkId { get; set; }

        [JsonProperty("mugshot_url")]
        public Uri MugshotUrl { get; set; }

        [JsonProperty("mugshot_url_template")]
        public string MugshotUrlTemplate { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("web_url")]
        public Uri WebUrl { get; set; }

        [JsonProperty("activated_at")]
        public string ActivatedAt { get; set; }

        [JsonProperty("auto_activated")]
        public bool AutoActivated { get; set; }

        [JsonProperty("stats")]
        public StatsClass Stats { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        public partial class StatsClass
        {
            [JsonProperty("following")]
            public long Following { get; set; }

            [JsonProperty("followers")]
            public long Followers { get; set; }

            [JsonProperty("updates")]
            public long Updates { get; set; }
        }
    }

    public partial class User
    {
        public static User FromJson(string json) => JsonConvert.DeserializeObject<User>(json, YammerScraper.Converter.Settings);
    }
}
