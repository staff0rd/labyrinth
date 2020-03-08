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
        [Route("user")]
        public async Task<Result<UserCard>> GetUser([FromBody] YammerUserQuery request)
        {
            var user = await _mediator.Send(request);
            
            return new Result<UserCard>(UserCard.FromUser(user.Response));
        }

        [HttpPost]
        [Route("backfill")]
        public QueuedJob Backfill([FromBody] TokenRequest request)
        {
            _credentials.Add(Network.Yammer, new Credential {
                Username = request.Username,
                Password = request.Password,
                ExternalIdentifier = request.Token
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
        [Route("users")]
        public async Task<Result<PagedResult<UserCard>>> Users([FromBody] SearchRequest request)
        {
            var result = await _mediator.Send(new YammerUsersQuery { 
                Username = request.Username, 
                Password = request.Password, 
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search,
            });

            return Map(result, UserCard.FromUser);
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

        private Result<PagedResult<TResult>> Map<T, TResult>(Result<PagedResult<T>> result, Func<T, TResult> mapper)
        {
            if (result.IsError)
                return new Result<PagedResult<TResult>> { IsError = true, Message = result.Message }; 

            var mapped = result.Response.Rows.Select(mapper).ToArray();
            return new Result<PagedResult<TResult>>(new PagedResult<TResult> {
                Rows = mapped,
                Page = result.Response.Page,
                PageSize = result.Response.PageSize,
                TotalRows = result.Response.TotalRows,
            });
        }
    }
}
