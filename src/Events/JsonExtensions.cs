using System;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Events
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static Event ToEvent(this string json, Guid sourceId, string entityId, string eventName) {
            return new Event {
                SourceId = sourceId,
                EntityId = entityId,
                EventName = eventName,
                Body = json,
            };
        }
    }
}