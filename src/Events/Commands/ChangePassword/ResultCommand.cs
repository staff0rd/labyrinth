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
            try
            {
                var result = await mediator.Send(this);
                if (result.IsError)
                    logger.LogError(result.Message);
                else
                    DisplayResult(result.Response);
            } 
            catch (Exception e)
            {
                logger.LogError(e, "Failed to execute command");
            }
        }
    }

    public abstract class ResultCommand : IRequest<Result>
    {
        protected async Task OnExecuteAsync(IMediator mediator, ILogger<ResultCommand> logger)
        {
            try
            {
                var result = await mediator.Send(this);
                if (result.IsError)
                    logger.LogError(result.Message);
                else
                    logger.LogInformation("Success");
            } 
            catch (Exception e)
            {
                logger.LogError(e, "Failed to execute command");
            }
        }
    }
}