using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Events
{
    public class GetImagesQueryHandler : IRequestHandler<GetImagesQuery, Result<PagedResult<Image>>>
    {
        private readonly IMediator _mediator;
        private readonly Store _store;

        public GetImagesQueryHandler(IMediator mediator, Store store)
        {
            _mediator = mediator;
            _store = store;
        }

        public async Task<Result<PagedResult<Image>>> Handle(GetImagesQuery request, CancellationToken cancellationToken)
        {
            var auth = await _mediator.Send(new AuthorizeQuery { Username = request.Username, Password = request.Password});
            if (auth.IsError) {
                return new Result<PagedResult<Image>> { IsError = true, Message = auth.Message};
            }
            
            var images = _store.GetImages(request.SourceId)
                .OrderByDescending(p => p.Created);

            return new Result<PagedResult<Image>>(images.GetPagedResult<Image>(request.PageNumber, request.PageSize));
        }
    }
}