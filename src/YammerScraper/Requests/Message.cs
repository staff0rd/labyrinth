// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using YammerScraper;
//
//    var message = Message.FromJson(jsonString);

namespace YammerScraper
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Message
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("sender_id")]
        public long SenderId { get; set; }

        [JsonProperty("replied_to_id")]
        public long? RepliedToId { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("network_id")]
        public long NetworkId { get; set; }

        [JsonProperty("message_type")]
        public string MessageType { get; set; }

        [JsonProperty("sender_type")]
        public string SenderType { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("web_url")]
        public Uri WebUrl { get; set; }

        [JsonProperty("group_id")]
        public long GroupId { get; set; }

        [JsonProperty("body")]
        public Body Body { get; set; }

        [JsonProperty("thread_id")]
        public long ThreadId { get; set; }

        [JsonProperty("client_type")]
        public string ClientType { get; set; }

        [JsonProperty("client_url")]
        public Uri ClientUrl { get; set; }

        [JsonProperty("system_message")]
        public bool SystemMessage { get; set; }

        [JsonProperty("direct_message")]
        public bool DirectMessage { get; set; }

        [JsonProperty("chat_client_sequence")]
        public object ChatClientSequence { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("notified_user_ids")]
        public long[] NotifiedUserIds { get; set; }

        [JsonProperty("privacy")]
        public string Privacy { get; set; }

        [JsonProperty("attachments")]
        public object[] Attachments { get; set; }

        [JsonProperty("liked_by")]
        public LikedBy LikedBy { get; set; }

        [JsonProperty("content_excerpt")]
        public string ContentExcerpt { get; set; }

        [JsonProperty("group_created_id")]
        public long? GroupCreatedId { get; set; }
    }

    public partial class Body
    {
        [JsonProperty("parsed")]
        public string Parsed { get; set; }

        [JsonProperty("plain")]
        public string Plain { get; set; }

        [JsonProperty("rich")]
        public string Rich { get; set; }
    }

    public partial class LikedBy
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("names")]
        public object[] Names { get; set; }
    }

    public partial class Message
    {
        public static Message FromJson(string json) => JsonConvert.DeserializeObject<Message>(json, YammerScraper.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Message self) => JsonConvert.SerializeObject(self, YammerScraper.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
