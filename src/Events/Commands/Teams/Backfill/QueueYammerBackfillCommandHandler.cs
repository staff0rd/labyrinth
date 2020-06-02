using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class QueueTeamsBackfillCommandHandler : IRequestHandler<QueueTeamsBackfillCommand>
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public QueueTeamsBackfillCommandHandler(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        public async Task<Unit> Handle(QueueTeamsBackfillCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username);
            await _mediator.Send(new TeamsBackfillCommand
            { 
                LabyrinthPassword = credential.Password,
                LabyrinthUsername = request.Username,
                SourceId = request.SourceId,
                Token = credential.ExternalSecret,
            });

            return Unit.Value;
        }
    }
}