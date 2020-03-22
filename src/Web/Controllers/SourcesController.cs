using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SourcesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public SourcesController(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        [HttpPost]
        public async Task<Result<List<Source>>> Get([FromBody] GetSourcesQuery request)
        {
            var sources = await _mediator.Send(request);
            return sources;
        }

        [HttpPost]
        [Route("add")]
        public async Task<Result> Add([FromBody] AddSourceCommand request)
        {   
            await _mediator.Send(request);
            return Result.Ok();
        }

        [HttpGet]
        [Route("networks")]
        public string[] GetNetworks()
        {
            return System.Enum.GetNames( typeof( Network ) )
                .Where(n => n != "Self")
                .ToArray();
        }
    }
}
