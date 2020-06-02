using MediatR;

namespace Events
{
    public class HydrateCommand : IRequest
    {
        public string LabyrinthUsername { get; set; }
        public string LabyrinthPassword { get; set; }
    }
}