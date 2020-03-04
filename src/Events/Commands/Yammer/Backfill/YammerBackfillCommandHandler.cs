using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Rest.Yammer;

namespace Events
{
    public class YammerBackfillCommandHandler : IRequestHandler<YammerBackfillCommand>
    {
        private readonly RestEventManager _rest;
        private readonly ILogger<YammerBackfillCommandHandler> _logger;
        private readonly CredentialCache _credentials;

        public YammerBackfillCommandHandler(ILogger<YammerBackfillCommandHandler> logger, RestEventManager rest,
            CredentialCache credentials) {
            _rest = rest;
            _logger = logger;
            _credentials = credentials;
        }

        public async Task<Unit> Handle(YammerBackfillCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (_credentials.Yammer.TryGetValue(request.Username, out var credential))
                {
                    long? last = null;
                    do 
                    {
                        var queryString = new { older_than = last };

                        var response = await _rest.Get(credential.Username, credential.Password, Network.Yammer, new MessagesSentRequest(_logger, YammerLimits.RateLimits), queryString, credential.Token);
                        if (response != null) {
                            last = response.Messages.Last()?.Id;
                            _logger.LogInformation("Found {count} messages, last is {last}", response.Messages.Count(), last);
                        } else {
                            last = null;
                        }
                    } while(last != null);
                }
            } catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }

            return new Unit();
        }
    }
}