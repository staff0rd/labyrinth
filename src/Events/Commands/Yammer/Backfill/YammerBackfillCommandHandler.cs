using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Events
{
    public class YammerBackfillCommandHandler : IRequestHandler<YammerBackfillCommand, Result>
    {
        private readonly RestEventManager _rest;
        private readonly ILogger<YammerBackfillCommandHandler> _logger;

        public YammerBackfillCommandHandler(ILogger<YammerBackfillCommandHandler> logger, RestEventManager rest) {
            _rest = rest;
            _logger = logger;
        }

        public async Task<Result> Handle(YammerBackfillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                long? last = null;
                do 
                {
                    var queryString = new { older_than = last };

                    var response = await _rest.Get(new Credential(request.Username, request.Password), request.SourceId, new MessagesSentRequest(_logger, YammerLimits.RateLimits), queryString, request.Token);
                    if (response != null) {
                        last = response.Messages.Last()?.Id;
                        _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                    } else {
                        last = null;
                    }
                } while(last != null);
                return Result.Ok();
            } catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                return Result.Error(e.Message);
            }
        }
    }
}