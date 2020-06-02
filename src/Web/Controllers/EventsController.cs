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
        public Task<Result<Overview>> Overview([FromBody] OverviewQuery request)
        {
            return _mediator.Send(request);
        }

        [HttpPost]
        [Route("hydrate")]
        public QueuedJob Hydrate([FromBody] QueryRequest request)
        {
            _credentials.Add(Network.Self.ToString(), new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new HydrateCommand { LabyrinthUsername = request.Username });
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
        public async Task<Result<PagedResult<UserCard>>> Users([FromBody] GetUsersQuery request)
        {
            var result = await _mediator.Send(request);

            return Map(result, UserCard.FromUser);
        }

        [HttpPost]
        [Route("events")]
        public async Task<Result<NextPagedResult<Event>>> Events([FromBody] GetEventsQuery request)
        {
            return await _mediator.Send(request);
        }

        [HttpPost]
        [Route("messages")]
        public async Task<Result<PagedResult<Events.Message>>> Messages([FromBody] GetMessagesQuery request)
        {
            return await _mediator.Send(request);
        }

        [HttpGet]
        [Route("image/{sourceId}/{id}")]
        public async Task<FileResult> Image([FromRoute] Guid sourceId, [FromRoute] Guid id )
        {
            var bytes = await System.IO.File.ReadAllBytesAsync(System.IO.Path.Combine(
                TeamsBackfillCommandHandler.GetImageDirectory(sourceId),
                id.ToString()
            ));
            return File(bytes, "image/jpg");
        }

        [HttpPost]
        [Route("images")]
        public async Task<Result<PagedResult<Events.Image>>> Images([FromBody] GetImagesQuery request)
        {
            return await _mediator.Send(request);
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
