using Events;

namespace Web.Controllers
{
    public class EventsRequest : QueryRequest
    {
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int LastId { get; set;}
        public Network Network { get; set;}
    }
}
