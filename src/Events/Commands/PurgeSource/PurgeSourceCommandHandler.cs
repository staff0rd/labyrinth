using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Events
{
    public class PurgeSourceCommandHandler : IRequestHandler<PurgeSourceCommand, Result>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PurgeSourceCommandHandler> _logger;

        public PurgeSourceCommandHandler(IMediator mediator, ILogger<PurgeSourceCommandHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Result> Handle(PurgeSourceCommand request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new PurgeEventsCommand { Username = request.Username, Password = request.Password, Events = new string[0], SourceId = request.SourceId });
            if (!result.IsError)
            {
                _logger.LogInformation("Deleting images...");
                var dir = TeamsBackfillCommandHandler.GetImageDirectory(request.SourceId);
                Directory.Delete(dir, true);
                _logger.LogInformation("Images deleted");
            }
            return result;
        }
    }
}