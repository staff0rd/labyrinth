using System;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.Yammer;
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
        [Route("overview")]
        public Task<Result<Overview>> Overview([FromBody] UserCredentialRequest request)
        {
            return _mediator.Send(new YammerOverviewQuery { Username = request.Username, Password = request.Password});
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] UserCredentialRequest request)
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

        [HttpPost]
        [Route("hydrate")]
        public QueuedJob Hydrate([FromBody] UserCredentialRequest request)
        {
            _credentials.Add(Network.Yammer, new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new HydrateCommand { Username = request.Username });
        }
    }
}
