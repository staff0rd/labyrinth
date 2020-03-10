using System;
using System.Threading.Tasks;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Web {
    public class QueuedJob
    {
        public string Id { get; set; }
        public string Command { get; set; }
    }
    public static class MediatRExtension
    {
        public static QueuedJob Enqueue(this IMediator mediator, IRequest request)
        {
            return new QueuedJob 
            {
                Id = BackgroundJob.Enqueue<HangfireMediator>(m => m.SendCommand(request)),
                Command = request.GetType().FullName
            };
        }
    }

    [LogToHangfireConsole]
    [ProgressToHangfireConsole]
    public class HangfireMediator
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HangfireMediator> _logger;

        public HangfireMediator(IMediator mediator, ILogger<HangfireMediator> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task SendCommand(IRequest request)
        {
            try {
                await _mediator.Send(request);
            } catch (Exception e) {
                _logger.LogError(e, e.ToString());
            }
        }
    }
}