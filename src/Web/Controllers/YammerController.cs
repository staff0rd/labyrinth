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
        public async Task<PagedResult<UserCard>> Get(int pageNumber = 0, int pageSize = 20, string search = "")
        {
            var users = await new GetUsers().Get(_events);
            if (!string.IsNullOrWhiteSpace(search))
            {
                Func<string, string> ifNull = (string a) => a == null ? "" : a;
                users = users.Where(u => ifNull(u.Name).ToLower().Contains(search.ToLower()) || ifNull(u.JobTitle).ToLower().Contains(search.ToLower())).ToArray();
            }

            return new PagedResult<UserCard>
            {
                Page = pageNumber,
                PageSize = pageSize,
                Rows = users
                    .Skip(pageNumber * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserCard
                    {
                        AvatarUrl = ScaleMugshot(u),
                        Id = u.Id.ToString(),
                        Network = "Yammer",
                        Name = u.FullName,
                        Description = u.JobTitle,
                    })
                    .ToArray(),
                TotalRows = users.Count()
            };
        }

        private static string ScaleMugshot(User u)
        {
            var result = u.MugshotUrl.ToString();
            if (string.IsNullOrWhiteSpace(result))
                return result;

            return result.Replace("48x48", "128x128");
        }
    }
}
