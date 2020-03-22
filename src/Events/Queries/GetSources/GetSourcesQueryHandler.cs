using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetSourcesQueryHandler : IRequestHandler<GetSourcesQuery, Result<List<Source>>>
    {
        private readonly KeyRepository _keys;
        private readonly IMediator _mediator;
        private readonly SourceRepository _sources;

        public GetSourcesQueryHandler(IMediator mediator, KeyRepository keys, SourceRepository sources)
        {
            _keys = keys;
            _mediator = mediator;
            _sources = sources;
        }

        public async Task<Result<List<Source>>> Handle(GetSourcesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<List<Source>> { IsError = true, Message = auth.Message};
            }

            var sources = await _sources.Get(new Credential { Username = request.Username, Password = request.Password});

            return new Result<List<Source>>(sources);
        }
    }
}