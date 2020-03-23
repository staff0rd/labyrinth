using System;
using System.Linq;
using System.Threading.Tasks;
using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YammerController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly CredentialCache _credentials;

        public YammerController(IMediator mediator, CredentialCache credentials)
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

            return _mediator.Enqueue(new YammerBackfillCommand { Username = request.Username, SourceId = request.SourceId });
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] QueryRequest request)
        {
            _credentials.Add(request.SourceId, new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new YammerProcessCommand { Username = request.Username, SourceId = request.SourceId });
        }
    }
}
