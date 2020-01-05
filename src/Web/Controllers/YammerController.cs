using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.Yammer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class YammerController : ControllerBase
    {
        private readonly ILogger<YammerController> _logger;

        private readonly EventStoreManager _events;

        public YammerController(ILogger<YammerController> logger, EventStoreManager events)
        {
            _logger = logger;
            _events = events;
        }

        [HttpGet]
        [Route("messages")]
        public async Task<PagedResult<Message>> GetMessages(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var messages = await new GetMessages().Get(_events);
            var result = messages.ToArray().GetPagedResult(pageNumber, pageSize, (m) => {
                return m;
            });
            return result;
        }

        [HttpGet]
        [Route("")]
        public async Task<Overview> GetOverview()
        {
            var overview = await new GetOverview().Get(_events);
            return overview;
        }

        [HttpGet]
        [Route("users/{id}")]
        public async Task<UserCard> GetUser(string id) {
            var user = await new GetUserById().Get(_events, id);
            return UserCard.FromUser(user);
        }

        [HttpGet]
        [Route("users")]
        public async Task<PagedResult<UserCard>> GetUsers(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var users = await new GetUsers().Get(_events);
            if (!string.IsNullOrWhiteSpace(search))
            {
                Func<string, string> ifNull = (string a) => a == null ? "" : a;
                users = users.Where(u => ifNull(u.Name).ToLower().Contains(search.ToLower()) || ifNull(u.JobTitle).ToLower().Contains(search.ToLower())).ToArray();
            }

            return users.GetPagedResult(pageNumber, pageSize, (u) => UserCard.FromUser(u));
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
