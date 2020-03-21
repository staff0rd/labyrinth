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
            _credentials.Add(Network.Yammer, new Credential {
                Username = request.Username,
                Password = request.Password,
                ExternalSecret = request.Token
            });

            return _mediator.Enqueue(new YammerBackfillCommand { Username = request.Username });
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] QueryRequest request)
        {
            _credentials.Add(Network.Yammer, new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new YammerProcessCommand { Username = request.Username });
        }

        

        [HttpPost]
        [Route("messages")]
        public async Task<Result<PagedResult<Events.Message>>> Messages([FromBody] SearchRequest request)
        {
            return await _mediator.Send(new YammerMessagesQuery { 
                Username = request.Username, 
                Password = request.Password, 
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search,
            });
        }
    }
}
