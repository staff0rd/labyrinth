using Hangfire;
using MediatR;

namespace Web {

    public static class MediatRExtension
    {
        public static void Enqueue(this IMediator mediator, IRequest request)
        {
            BackgroundJob.Enqueue<HangfireMediator>(m => m.Send(request));
        }
    }

    public class HangfireMediator
    {
        private readonly IMediator _mediator;

        public HangfireMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public void Send(IRequest request)
        {
            _mediator.Send(request);
        }
    }
}