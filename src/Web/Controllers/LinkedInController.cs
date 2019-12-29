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
        public async Task<User[]> Get()
        {
            return await new Queries(_events).GetUsers();
        }
    }
}
