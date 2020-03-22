using System;
using Newtonsoft.Json;

namespace Events
{
    public class Event : IEntity<int>
    {
        public int Id { get; set; }
        public string EntityId { get; set;}
        public string EventName { get; set;}
        public Guid SourceId { get; set; }
        public string Body { get; set; }
        public DateTime InsertedAt { get; set;}
        public long TimeStamp { get; set;}
    }
}