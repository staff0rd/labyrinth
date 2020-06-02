using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class PurgeSourceCommandHandler : IRequestHandler<PurgeSourceCommand, Result>
    {
        private readonly IMediator _mediator;

        public PurgeSourceCommandHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<Result> Handle(PurgeSourceCommand request, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new PurgeEventsCommand { Username = request.Username, Password = request.Password, Events = new string[0], SourceId = request.SourceId });
        }
    }
}