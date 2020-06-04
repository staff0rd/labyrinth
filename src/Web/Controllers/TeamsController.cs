using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TeamsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public TeamsController(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        [HttpPost]
        [Route("backfill")]
        public QueuedJob Backfill([FromBody] TokenRequest request)
        {
            _credentials.Add(request.SourceId, new Credential {
                Username = request.Username,
                Password = request.Password,
                ExternalSecret = request.Token
            });

            return _mediator.Enqueue(new TeamsBackfillCommand { LabyrinthUsername = request.Username, SourceId = request.SourceId });
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] TokenRequest request)
        {
            _credentials.Add(request.SourceId, new Credential {
                Username = request.Username,
                Password = request.Password,
                ExternalSecret = request.Token,
            });

            return _mediator.Enqueue(new QueueTeamsProcessCommand { LabyrinthUsername = request.Username, SourceId = request.SourceId });
        }
    }
}