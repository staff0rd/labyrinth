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

        public YammerController(IMediator mediator, CredentialCache credentials)
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
        // [Route("")]
        // public Overview GetOverview()
        // {
        //     var overview = _store.GetOverview().FirstOrDefault(x => x.Network == Network.Yammer);
        //     return overview;
        // }

        // [HttpGet]
        // [Route("users/{id}")]
        // public UserCard GetUser(string id) {
        //     var user = _store.GetUsers(Network.Yammer).FirstOrDefault(p => p.Id == id);
        //     return UserCard.FromUser(user);
        // }

        // [HttpGet]
        // [Route("users")]
        // public PagedResult<UserCard> GetUsers(int pageNumber = 0, int pageSize = 20, string search = "")
        // {
        //     var users = _store.GetUsers(Network.Yammer);
        //     if (!string.IsNullOrWhiteSpace(search))
        //     {
        //         Func<string, string> ifNull = (string a) => a == null ? "" : a;
        //         users = users.Where(u => ifNull(u.Name).ToLower().Contains(search.ToLower()) || ifNull(u.Description).ToLower().Contains(search.ToLower())).ToArray();
        //     }

        //     return users.GetPagedResult(pageNumber, pageSize, (u) => UserCard.FromUser(u));
        // }
        public class BackfillYammerRequest
        {
            public string Username { get; set; }
            public string Token { get; set; }
            public string Password { get; set; }
        }

        public class ProcessYammerRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("backfill")]
        public QueuedJob Backfill([FromBody] BackfillYammerRequest request)
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
        public QueuedJob Backfill([FromBody] ProcessYammerRequest request)
        {
            _credentials.Yammer.TryRemove(request.Username, out var _);
            _credentials.Yammer.TryAdd(request.Username, new YammerCredential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new YammerProcessCommand { Username = request.Username });
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
