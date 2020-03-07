using System;
using System.Threading;
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

    public void New() {
        if (PerformContext.Value == null)
            return;
        _progress = PerformContext.Value.WriteProgressBar(0);
    }

    public void Set(int value)
    {
        if (_progress == null)
            New();

        if (PerformContext.Value == null)
            return;
        _progress.SetValue(value);
    }

    public void Set(int current, int total)
    {
        int progress = current * 100 / total;
        Set(progress);
    }
}
