using System.Threading.Tasks;
using Hangfire;
using MediatR;

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
    public class HangfireMediator
    {
        private readonly IMediator _mediator;

        public HangfireMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task SendCommand(IRequest request)
        {
            await _mediator.Send(request);
        }
    }
}