using System;

namespace Events
{
    public class Overview
    {
         public Guid SourceId { get; set;}
         public int Messages { get; set; }
         public int Users { get; set; }
         public int Topics { get; set; }
         public int Groups { get; set; }
         public int Images { get; set; }
         public EventCount[] Events { get; set;}
    }
}