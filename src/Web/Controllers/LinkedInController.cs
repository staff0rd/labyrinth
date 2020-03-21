using System;
using System.Threading.Tasks;
using Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LinkedInController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly CredentialCache _credentials;

        public LinkedInController(IMediator mediator, CredentialCache credentials)
        {
            _mediator = mediator;
            _credentials = credentials;
        }

        [HttpPost]
        [Route("backfill")]
        public QueuedJob Backfill([FromBody] CredentialRequest request)
        {
            _credentials.Add(Network.LinkedIn, new Credential {
                Username = request.Username,
                Password = request.Password,
                ExternalIdentifier = request.ExternalIdentifier,
                ExternalSecret = request.ExternalSecret
            });

            return _mediator.Enqueue(new LinkedInBackfillCommand { Username = request.Username });
        }

        [HttpPost]
        [Route("process")]
        public QueuedJob Process([FromBody] QueryRequest request)
        {
            _credentials.Add(Network.LinkedIn, new Credential {
                Username = request.Username,
                Password = request.Password
            });

            return _mediator.Enqueue(new LinkedInProcessCommand { Username = request.Username });
        }
    }
}

//         [HttpGet]
//         public async Task<PagedResult<UserCard>> Get(int pageNumber = 0, int pageSize = 20, string search = "")
//         {
//             var users = _store.Users.Select(x => x.Value);
//             if (!string.IsNullOrWhiteSpace(search))
//             {
//                 users = users.Where(u => u.Name.ToLower().Contains(search.ToLower()) || u.Occupation.ToLower().Contains(search.ToLower())).ToArray();
//             }
//             return new PagedResult<UserCard> {
//                 Page = pageNumber,
//                 PageSize = pageSize,
//                 Rows = users
//                     .Skip(pageNumber * pageSize)
//                     .Take(pageSize)
//                     .Select(u => new UserCard {
//                         AvatarUrl = u.MugshotUrl,
//                         Description = u.Occupation,
//                         Id = u.Id,
//                         Name = u.Name,
//                         Network = "LinkedIn",
//                         KnownSince = u.Connected
//                     })
//                     .ToArray(),
//                 TotalRows = users.Count()
//             };
//         }
//     }
// }
