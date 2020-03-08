using System;
using MediatR;

namespace Events
{
    public class YammerMessagesQuery : IRequest<Result<PagedResult<Message>>>
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Search { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}