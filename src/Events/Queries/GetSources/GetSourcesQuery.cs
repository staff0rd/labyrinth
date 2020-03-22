using System.Collections.Generic;
using MediatR;

namespace Events
{
    public class GetSourcesQuery : IRequest<Result<List<Source>>>
    {
        public string Username {get; set;}
        public string Password { get; set;}
    }
}