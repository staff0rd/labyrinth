using System.Collections.Generic;

namespace YammerScraper
{
    public class MessagesSentReferences {
        public List<Message> Messages { get; set;} = new List<Message>();
        public List<User> Users { get; set;} = new List<User>();
        public List<Group> Groups { get; set;} = new List<Group>();
        public List<Thread> Threads { get; set; } = new List<Thread>();
    }
    public class MessagesSentResponse {
        public List<Message> Messages { get; set;}

        public MessagesSentReferences References { get; set; }

        public MessagesSentResponse() {
            References = new MessagesSentReferences();
            Messages = new List<Message>();
        } 
    }
}