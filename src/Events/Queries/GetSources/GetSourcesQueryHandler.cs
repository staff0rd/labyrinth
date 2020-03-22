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
        private readonly Store _store;

        public GetSourcesQueryHandler(IMediator mediator, KeyRepository keys, SourceRepository sources, Store store)
        {
            _keys = keys;
            _mediator = mediator;
            _sources = sources;
            _store = store;
        }

        public async Task<Result<List<Source>>> Handle(GetSourcesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<List<Source>> { IsError = true, Message = auth.Message};
            }
            var creds = new Credential { Username = request.Username, Password = request.Password};
            var sources = await _sources.Get(creds);

            _store.UpdateSources(sources);

            return new Result<List<Source>>(sources);
        }
    }
}