using Newtonsoft.Json;

namespace Rest
{
    public class RestApiRequest
    {
        public string Uri { get; set; }
        [JsonConverter(typeof(RawConverter))]
        public string Data { get; set; }
        public string Method { get; set;}
        public string Category { get; set; }
        [JsonConverter(typeof(RawConverter))]
        public string Response { get; set; }
    }
}