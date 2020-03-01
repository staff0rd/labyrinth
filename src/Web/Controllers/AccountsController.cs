using System.Threading.Tasks;
using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Get() {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAccountCommand request)
        {
            var response = await _mediator.Send(request);
            if (response.IsError)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpPost]
        [Route("change-password")]
        public async Task ChangePassword([FromBody] ChangePasswordRequest request) {
            // await _keys.ChangePassword(request.UserName, request.OldPassword, request.NewPassword);
        }
    }
}