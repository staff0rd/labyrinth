using System;

namespace Events.TeamsWeb
{
    public class ChatOverview
    {
        public string Author { get; set; }
        public DateTimeOffset? Time { get; set; }
        public string LastMessage { get; set; }
    }
}