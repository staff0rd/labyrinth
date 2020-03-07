using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.Yammer;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YammerController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly CredentialCache _credentials;

        public YammerController(IMediator mediator, CredentialCache credentials, Store store)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        // [HttpGet]
        // [Route("messages")]
        // public PagedResult<Events.Message> GetMessages(int pageNumber = 0, int pageSize = 20, string search = "")
        // {
        //     var messages = _store.GetMessages(Network.Yammer)
        //         .OrderByDescending(p => p.CreatedAt);
                
        //     var result = messages.GetPagedResult(pageNumber, pageSize, (m) => {
        //         return m;
        //     });
        //     return result;
        // }

        // [HttpGet]
        // [Route("users/{id}")]
        // public UserCard GetUser(string id) {
        //     var user = _store.GetUsers(Network.Yammer).FirstOrDefault(p => p.Id == id);
        //     return UserCard.FromUser(user);
        // }

        public class YammerCredentialRequest : UserCredentialRequest
        {
            public string Token { get; set; }
        }

        public class SearchRequest : UserCredentialRequest
        {
            public string Search { get; set; }

            public int PageSize { get; set; }

            public int PageNumber { get; set;}
        }

        public class UserCredentialRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("backfill")]
        public QueuedJob Backfill([FromBody] YammerCredentialRequest request)
        {
            _credentials.Yammer.TryRemove(request.Username, out var _);
            _credentials.Yammer.TryAdd(request.Username, new YammerCredential {
                Username = request.Username,
                Password = request.Password,
                Token = request.Token
            });

            return _mediator.Enqueue(new YammerBackfillCommand { Username = request.Username });
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] UserCredentialRequest request)
        {
            _credentials.Yammer.TryRemove(request.Username, out var _);
            _credentials.Yammer.TryAdd(request.Username, new YammerCredential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new YammerProcessCommand { Username = request.Username });
        }

        [HttpPost]
        [Route("overview")]
        public Task<Result<Overview>> Overview([FromBody] UserCredentialRequest request)
        {
            return _mediator.Send(new YammerOverviewQuery { Username = request.Username, Password = request.Password});
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
            
            if (result.IsError)
                return new Result<PagedResult<UserCard>> { IsError = true, Message = result.Message }; 

            var mapped = result.Response.Rows.Select(UserCard.FromUser).ToArray();
            return new Result<PagedResult<UserCard>>(new PagedResult<UserCard> {
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
            _credentials.Yammer.TryRemove(request.Username, out var _);
            _credentials.Yammer.TryAdd(request.Username, new YammerCredential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new HydrateCommand { Username = request.Username });
        }

        private static PagedResult<TResult> PagedResult<TCollection, TResult>(TCollection[] items, int pageNumber, int pageSize, Func<TCollection, TResult> selector)
        {
            return new PagedResult<TResult>
            {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = items
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(selector)
                    .ToArray(),
                TotalRows = items.Count()
            };
        }
    }
}
