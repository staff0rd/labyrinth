using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Events;
using Events.LinkedIn;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinkedInController : ControllerBase
    {
        private readonly ILogger<LinkedInController> _logger;

        private readonly EventStoreManager _events;

        public LinkedInController(ILogger<LinkedInController> logger, EventStoreManager events)
        {
            _logger = logger;
            _events = events;
        }

        [HttpGet]
        public async Task<PagedResult<User>> Get(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var users = await new Queries(_events).GetUsers();
            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(u => u.Name.ToLower().Contains(search.ToLower()) || u.Occupation.ToLower().Contains(search.ToLower())).ToArray();
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
