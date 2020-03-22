using System;

namespace Events
{
    public class Overview
    {
         public Guid SourceId { get; set;}
         public int Messages { get; set; }
         public int Users { get; set; }
         public int Threads { get; set; }
         public int Groups { get; set; }
         public EventCount[] Events { get; set;}
    }
}