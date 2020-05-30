using System;
using System.Threading;
using System.Threading.Tasks;
using Events;
using Hangfire.Console;
using Hangfire.Console.Progress;
using Hangfire.Server;

public class HangfireConsoleProgress : IProgress
{
    private class AsyncLocalScope : IDisposable
    {
        public AsyncLocalScope(PerformContext context) => PerformContext.Value = context;
        public void Dispose() => PerformContext.Value = null;
    }

    private static readonly AsyncLocal<PerformContext> PerformContext = new AsyncLocal<PerformContext>();
    private IProgressBar _progress;

    public static IDisposable InContext(PerformContext context) => new AsyncLocalScope(context);

    public IDisposable BeginScope<TState>(TState state) => null;

    public Task New() {
        if (PerformContext.Value != null)
            _progress = PerformContext.Value.WriteProgressBar(0);
        return Task.CompletedTask;
    }

    public Task Set(int value)
    {
        if (_progress == null)
            New();

        if (PerformContext.Value != null)
            _progress.SetValue(value);

        return Task.CompletedTask;
    }

    public async Task Set(int current, int total)
    {
        int progress = current * 100 / total;
        await Set(progress);
    }
}
