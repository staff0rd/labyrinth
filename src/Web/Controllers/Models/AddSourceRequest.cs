using System;
using Events;

namespace Web.Controllers
{
    public class AddSourceRequest : QueryRequest
    {
        public Guid Id { get; set; }
        public Network Network { get; set; }
        public string Name { get; set; }
    }
}