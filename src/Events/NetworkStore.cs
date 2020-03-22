using System.Collections.Generic;

namespace Events
{
    public class NetworkStore
    {
        public Dictionary<string, Message> Messages { get; set; } = new Dictionary<string, Message>();
        public Dictionary<string, User> Users { get; set; } = new Dictionary<string, User>();

        public Dictionary<string, Topic> Topics { get; set; } = new Dictionary<string, Topic>();
    }
}