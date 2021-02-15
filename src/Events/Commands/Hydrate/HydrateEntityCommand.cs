using System;
using System.Collections.Generic;
using MediatR;

namespace Events
{
    public class HydrateEntityCommand<T> : IRequest where T : IExternalEntity
    {
        public HydrateEntityCommand(Credential credential, string entityType, Guid sourceId, string eventType, Dictionary<string, T> dictionary)
        {
            LabyrinthUsername = credential.Username;
            LabyrinthPassword = credential.Password;
            EntityType = entityType;
            SourceId = sourceId;
            EventType = eventType;
            Dictionary = dictionary;
        }

        public string LabyrinthUsername { get; set; }
        public string LabyrinthPassword { get; set; }
        public string EntityType { get; set; }
        public Guid SourceId { get; set; }
        public string EventType { get; set; }
        public Dictionary<string, T> Dictionary { get; set; }
    }
}