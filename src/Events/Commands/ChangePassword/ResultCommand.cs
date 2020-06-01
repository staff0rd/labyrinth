using MediatR;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace Events
{
    public abstract class ResultCommand<T> : IRequest<Result<T>>
    {
        protected abstract void DisplayResult(T result);

        protected async Task OnExecuteAsync(IMediator mediator, ILogger<ResultCommand> logger)
        {
            await ResultCommand.Execute<ResultCommand<T>, Result<T>>(this, mediator, logger, (result) => DisplayResult(result.Response));
        }
    }

    public abstract class ResultCommand : IRequest<Result>
    {
        protected async Task OnExecuteAsync(IMediator mediator, ILogger<ResultCommand> logger)
        {
            await Execute<ResultCommand, Result>(this, mediator, logger, (result) => logger.LogInformation("Success"));
        }

        public static async Task Execute<TCommand, TResult>(TCommand command, IMediator mediator, ILogger logger, Action<TResult> onSuccess)
            where TCommand : IRequest<TResult>
            where TResult: Result
        {
            try
            {
                var result = await mediator.Send(command);
                if (result.IsError)
                    logger.LogError(result.Message);
                else
                    onSuccess(result);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to execute command");
            }
        }
    }
}