using System;
using System.Text;
using EventStore.ClientAPI;
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
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static Event ToEvent(this string json, Network network, Guid entityId, string eventName) {
            return new Event {
                Network = network,
                EntityId = entityId,
                EventName = eventName,
                Body = json,
            };
        }
    }
}