using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

namespace Events
{
    public class Source : IEntity<Guid>
    {
        public Guid Id { get; set; }
        public string Name { get; set;}
        public Network Network { get; set; }
    }
}