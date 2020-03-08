using System;
using System.Threading.Tasks;
using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public EventsController(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        [HttpPost]
        [Route("overview")]
        public Task<Result<Overview>> Overview([FromQuery] Network network, [FromBody] QueryRequest request)
        {
            return _mediator.Send(new OverviewQuery { Username = request.Username, Password = request.Password, Network = network});
        }
    }
}
