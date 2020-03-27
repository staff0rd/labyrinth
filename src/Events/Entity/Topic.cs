using System;
using System.Collections.Generic;
using Microsoft.Graph;

namespace Events
{
    public class Topic : IExternalEntity
    {
        public string Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastUpdated { get; set; }
        public string Title { get; set;}
        public Network Network { get; set; }
        public Dictionary<string, User> Members { get; set; } = new Dictionary<string, User>();

        internal static Topic From(Chat chat)
        {
            return new Topic
            {
                Id = chat.Id,
                Network = Network.Teams,
            };
        }
    }
}