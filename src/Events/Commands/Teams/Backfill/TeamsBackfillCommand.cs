using System;
using MediatR;

namespace Events
{
    public class TeamsBackfillCommand : IRequest
    {
        public string LabyrinthUsername { get; set; }
        public Guid SourceId { get; set;}
    }
}