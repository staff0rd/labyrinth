namespace Events.TeamsWeb
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class FetchShortProfile
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value")]
        public Value[] Value { get; set; }
    }

    public partial class Value
    {
        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("jobTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string JobTitle { get; set; }

        [JsonProperty("userLocation", NullValueHandling = NullValueHandling.Ignore)]
        public string UserLocation { get; set; }

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

        [JsonProperty("userType", NullValueHandling = NullValueHandling.Ignore)]
        public string UserType { get; set; }

        [JsonProperty("isShortProfile", NullValueHandling = NullValueHandling.Ignore)]
        public bool? IsShortProfile { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("mri")]
        public string Mri { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("featureSettings", NullValueHandling = NullValueHandling.Ignore)]
        public FeatureSettings FeatureSettings { get; set; }
    }

    public partial class FeatureSettings
    {
        [JsonProperty("isPrivateChatEnabled")]
        public bool IsPrivateChatEnabled { get; set; }

        [JsonProperty("enableShiftPresence")]
        public bool EnableShiftPresence { get; set; }
    }
}
