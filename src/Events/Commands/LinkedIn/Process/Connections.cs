namespace Events.LinkedIn
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Connections
    {
        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("included")]
        public Included[] Included { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; }

        [JsonProperty("entityUrn")]
        public string EntityUrn { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("*elements")]
        public string[] Elements { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }

    public partial class Metadata
    {
        [JsonProperty("type")]
        public string MetadataType { get; set; }

        [JsonProperty("id")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Id { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }
    }

    public partial class Paging
    {
        [JsonProperty("count")]
        public long Count { get; set; }

        [JsonProperty("start")]
        public long Start { get; set; }

        [JsonProperty("links")]
        public object[] Links { get; set; }
    }

    public partial class Included
    {
        [JsonProperty("firstName", NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; }

        [JsonProperty("occupation", NullValueHandling = NullValueHandling.Ignore)]
        public string Occupation { get; set; }

        [JsonProperty("objectUrn", NullValueHandling = NullValueHandling.Ignore)]
        public string ObjectUrn { get; set; }

        [JsonProperty("entityUrn")]
        public string EntityUrn { get; set; }

        // [JsonProperty("backgroundImage")]
        // public BackgroundImage BackgroundImage { get; set; }

        [JsonProperty("publicIdentifier", NullValueHandling = NullValueHandling.Ignore)]
        public string PublicIdentifier { get; set; }

        // [JsonProperty("picture")]
        // public BackgroundImage Picture { get; set; }

        [JsonProperty("trackingId", NullValueHandling = NullValueHandling.Ignore)]
        public string TrackingId { get; set; }

        [JsonProperty("$type")]
        public string Type { get; set; }

        [JsonProperty("createdAt", NullValueHandling = NullValueHandling.Ignore)]
        public long? CreatedAt { get; set; }

        [JsonProperty("insights")]
        public object Insights { get; set; }

        [JsonProperty("*miniProfile", NullValueHandling = NullValueHandling.Ignore)]
        public string MiniProfile { get; set; }

        [JsonProperty("weChatContactInfo")]
        public object WeChatContactInfo { get; set; }

        [JsonProperty("phoneNumbers")]
        public object PhoneNumbers { get; set; }

        [JsonProperty("primaryEmailAddress")]
        public object PrimaryEmailAddress { get; set; }

        [JsonProperty("twitterHandles")]
        public object TwitterHandles { get; set; }
    }

    public partial class BackgroundImage
    {
        // [JsonProperty("artifacts")]
        // public Artifact[] Artifacts { get; set; }

        [JsonProperty("rootUrl")]
        public Uri RootUrl { get; set; }

        [JsonProperty("$type")]
        public BackgroundImageType Type { get; set; }
    }

    public partial class Artifact
    {
        [JsonProperty("width")]
        public long Width { get; set; }

        [JsonProperty("fileIdentifyingUrlPathSegment")]
        public string FileIdentifyingUrlPathSegment { get; set; }

        [JsonProperty("expiresAt")]
        public long ExpiresAt { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("$type")]
        public ArtifactType Type { get; set; }
    }

    public enum ArtifactType { ComLinkedinCommonVectorArtifact };

    public enum BackgroundImageType { ComLinkedinCommonVectorImage };

    public enum IncludedType { ComLinkedinVoyagerIdentitySharedMiniProfile, ComLinkedinVoyagerRelationshipsSharedConnectionConnection };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                IncludedTypeConverter.Singleton,
                BackgroundImageTypeConverter.Singleton,
                ArtifactTypeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class IncludedTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IncludedType) || t == typeof(IncludedType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "com.linkedin.voyager.identity.shared.MiniProfile":
                    return IncludedType.ComLinkedinVoyagerIdentitySharedMiniProfile;
                case "com.linkedin.voyager.relationships.shared.connection.Connection":
                    return IncludedType.ComLinkedinVoyagerRelationshipsSharedConnectionConnection;
            }
            throw new Exception("Cannot unmarshal type IncludedType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (IncludedType)untypedValue;
            switch (value)
            {
                case IncludedType.ComLinkedinVoyagerIdentitySharedMiniProfile:
                    serializer.Serialize(writer, "com.linkedin.voyager.identity.shared.MiniProfile");
                    return;
                case IncludedType.ComLinkedinVoyagerRelationshipsSharedConnectionConnection:
                    serializer.Serialize(writer, "com.linkedin.voyager.relationships.shared.connection.Connection");
                    return;
            }
            throw new Exception("Cannot marshal type IncludedType");
        }

        public static readonly IncludedTypeConverter Singleton = new IncludedTypeConverter();
    }

    internal class BackgroundImageTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(BackgroundImageType) || t == typeof(BackgroundImageType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "com.linkedin.common.VectorImage")
            {
                return BackgroundImageType.ComLinkedinCommonVectorImage;
            }
            throw new Exception("Cannot unmarshal type BackgroundImageType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (BackgroundImageType)untypedValue;
            if (value == BackgroundImageType.ComLinkedinCommonVectorImage)
            {
                serializer.Serialize(writer, "com.linkedin.common.VectorImage");
                return;
            }
            throw new Exception("Cannot marshal type BackgroundImageType");
        }

        public static readonly BackgroundImageTypeConverter Singleton = new BackgroundImageTypeConverter();
    }

    internal class ArtifactTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ArtifactType) || t == typeof(ArtifactType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "com.linkedin.common.VectorArtifact")
            {
                return ArtifactType.ComLinkedinCommonVectorArtifact;
            }
            throw new Exception("Cannot unmarshal type ArtifactType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ArtifactType)untypedValue;
            if (value == ArtifactType.ComLinkedinCommonVectorArtifact)
            {
                serializer.Serialize(writer, "com.linkedin.common.VectorArtifact");
                return;
            }
            throw new Exception("Cannot marshal type ArtifactType");
        }

        public static readonly ArtifactTypeConverter Singleton = new ArtifactTypeConverter();
    }
}
