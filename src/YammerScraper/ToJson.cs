using System;
using System.Text;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YammerScraper
{
    public static class JsonExtensions
    {
        public static string ToJson(this object obj)
        {
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return JsonConvert.SerializeObject(obj, settings);
        }

        public static EventData ToEvent(this string json, string eventName) {
            return new EventData(
                Guid.NewGuid(),
                eventName,
                true,
                Encoding.ASCII.GetBytes(json), 
                Encoding.ASCII.GetBytes(new { metadata = new { machine = Environment.MachineName }}.ToJson()));
        }
    }
}