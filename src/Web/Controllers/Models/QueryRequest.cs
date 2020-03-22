using System;

namespace Web.Controllers
{
    public class QueryRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public Guid SourceId { get; set;}
    }
}
