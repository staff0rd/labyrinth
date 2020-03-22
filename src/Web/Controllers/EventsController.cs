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
        public Task<Result<Overview>> Overview([FromQuery] Guid sourceId, [FromBody] QueryRequest request)
        {
            return _mediator.Send(new OverviewQuery { Username = request.Username, Password = request.Password, SourceId = sourceId});
        }

        [HttpPost]
        [Route("hydrate")]
        public QueuedJob Hydrate([FromBody] QueryRequest request)
        {
            _credentials.Add(Network.Self.ToString(), new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new HydrateCommand { Username = request.Username });
        }

        [HttpPost]
        [Route("user")]
        public async Task<Result<UserCard>> GetUser([FromBody] GetUserQuery request)
        {
            var user = await _mediator.Send(request);
            
            return new Result<UserCard>(UserCard.FromUser(user.Response));
        }

        [HttpPost]
        [Route("users")]
        public async Task<Result<PagedResult<UserCard>>> Users([FromBody] SearchRequest request)
        {
            var result = await _mediator.Send(new GetUsersQuery { 
                Username = request.Username, 
                Password = request.Password, 
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                Search = request.Search,
                SourceId = request.SourceId
            });

            return Map(result, UserCard.FromUser);
        }

        [HttpPost]
        [Route("events")]
        public async Task<Result<NextPagedResult<Event>>> Events([FromBody] EventsRequest request)
        {
            var result = await _mediator.Send(new GetEventsQuery { 
                Username = request.Username, 
                Password = request.Password, 
                LastId = request.LastId,
                PageSize = request.PageSize,
                Search = request.Search,
                SourceId = request.SourceId
            });

            return result;
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
