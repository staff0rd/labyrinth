using System;
using MediatR;

namespace Events
{
    public class GetUsersQuery : IRequest<Result<PagedResult<User>>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public Network Network { get; set; }
    }
}