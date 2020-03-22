using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Events
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Network
    {
        Self = 0,
        Yammer = 10,
        LinkedIn = 11,
        Teams = 12,
    }
}