using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class QueueYammerBackfillCommandHandler : IRequestHandler<QueueYammerBackfillCommand>
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public QueueYammerBackfillCommandHandler(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        public async Task<Unit> Handle(QueueYammerBackfillCommand request, CancellationToken cancellationToken)
        {
            var credential = _credentials.Get(request.SourceId, request.Username);
            await _mediator.Send(new YammerBackfillCommand
            { 
                Password = credential.Password,
                SourceId = request.SourceId,
                Token = credential.ExternalSecret,
                Username = request.Username,
            });

            return Unit.Value;
        }
    }
}