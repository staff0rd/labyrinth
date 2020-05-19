using System;
using System.Threading;
using System.Threading.Tasks;
using Events;
using Hangfire.Console;
using Hangfire.Console.Progress;
using Hangfire.Server;
using Microsoft.AspNetCore.SignalR;
using Web;

public class HangfireSignalRProgress : IProgress
{
    private readonly TaskHubSender _send;

    public HangfireSignalRProgress(TaskHubSender sender)
    {
        _send = sender;
    }

    private class AsyncLocalScope : IDisposable
    {
        public AsyncLocalScope(PerformContext context) => PerformContext.Value = context;
        public void Dispose() => PerformContext.Value = null;
    }

    private static readonly AsyncLocal<PerformContext> PerformContext = new AsyncLocal<PerformContext>();
    

    public static IDisposable InContext(PerformContext context) => new AsyncLocalScope(context);

    public IDisposable BeginScope<TState>(TState state) => null;

    public async Task New()
    {
        if (PerformContext.Value != null)
            await _send.Progress(PerformContext.Value.BackgroundJob.Id, 0);
    }

    public async Task Set(int value)
    {
        if (PerformContext.Value != null)
            await _send.Progress(PerformContext.Value.BackgroundJob.Id, value);
    }

    public async Task Set(int current, int total)
    {
        int progress = current * 100 / total;
        await Set(progress);
    }
}
