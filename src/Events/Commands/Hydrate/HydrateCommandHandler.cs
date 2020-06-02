using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class HydrateCommandHandler : IRequestHandler<HydrateCommand>
    {
        private readonly ILogger<HydrateCommandHandler> _logger;
        private readonly EventRepository _events;
        private readonly IProgress _progress;
        private readonly Store _store;
        private readonly IMediator _mediator;

        public HydrateCommandHandler(ILogger<HydrateCommandHandler> logger, Store store, IMediator mediator,
            EventRepository events, IProgress progress)
        {
            _logger = logger;
            _store = store;
            _events = events;
            _progress = progress;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(HydrateCommand request, CancellationToken cancellationToken)
        {
            var credential = new Credential(request.LabyrinthUsername, request.LabyrinthPassword);
            await _mediator.Send(new GetSourcesQuery { Username = request.LabyrinthUsername, Password = request.LabyrinthPassword });
            await _store.Hydrate(credential);
            return Unit.Value;
        }
    }
}