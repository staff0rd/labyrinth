using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class QueueHydrateCommandHandler : IRequestHandler<QueueHydrateCommand>
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public QueueHydrateCommandHandler(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        public async Task<Unit> Handle(QueueHydrateCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username);
            await _mediator.Send(new HydrateCommand
            { 
                LabyrinthPassword = credential.Password,
                LabyrinthUsername = request.Username,
            });

            return Unit.Value;
        }
    }
}