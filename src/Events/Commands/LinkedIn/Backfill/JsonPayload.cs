using Newtonsoft.Json;
using Rest;

namespace Events
{
    public class JsonPayload
    {
        public string Url { get; set; }
        [JsonConverter(typeof(RawConverter))]
        public string Json { get; set; }
    }
}