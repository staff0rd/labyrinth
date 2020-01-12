using System;
using Newtonsoft.Json;

namespace Events
{
    public class Event : IEntity<int>
    {
        public int Id { get; set; }
        public Guid EntityId { get; set;}
        public string EventName { get; set;}
        public Network Network { get; set; }
        public string Body { get; set; }
        public DateTime InsertedAt { get; set;}
    }
}