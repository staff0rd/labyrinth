using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class QueueTeamsProcessCommandHandler : IRequestHandler<QueueTeamsProcessCommand>
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public QueueTeamsProcessCommandHandler(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        public async Task<Unit> Handle(QueueTeamsProcessCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username);
            await _mediator.Send(new TeamsProcessCommand
            { 
                LabyrinthPassword = credential.Password,
                LabyrinthUsername = request.Username,
                SourceId = request.SourceId,
            });

            return Unit.Value;
        }
    }
}