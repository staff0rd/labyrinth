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
        public async Task<PagedResult<User>> Get(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var users = await new GetUsers().Get(_events);
            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u => u.Name.ToLower().Contains(search.ToLower()) || u.JobTitle.ToLower().Contains(search.ToLower())).ToArray();
            }
            return new PagedResult<User> {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = users
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .ToArray(),
                TotalRows = users.Count()
            };
        }
    }
}
