using System;
using System.Collections.Concurrent;
using System.Threading;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Logging;

public class HangfireConsoleLogger: ILogger
{
    private class AsyncLocalScope : IDisposable
    {
        public AsyncLocalScope(PerformContext context) => PerformContext.Value = context;
        public void Dispose() => PerformContext.Value = null;
    }

    private static readonly AsyncLocal<PerformContext> PerformContext = new AsyncLocal<PerformContext>();
    
    public static IDisposable InContext(PerformContext context) => new AsyncLocalScope(context);

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Debug;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (PerformContext.Value == null)
            return;

        var message = formatter(state, exception);

        if (logLevel == LogLevel.Error || logLevel == LogLevel.Critical)
            PerformContext.Value.SetTextColor(ConsoleTextColor.Red);
        if (logLevel == LogLevel.Warning)
            PerformContext.Value.SetTextColor(ConsoleTextColor.Yellow);

        PerformContext.Value.WriteLine(message);
        PerformContext.Value.ResetTextColor();
    }
}

public class HangfireConsoleLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, ILogger> _loggers = new ConcurrentDictionary<string, ILogger>();

    public void Dispose() => _loggers.Clear();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, name => new HangfireConsoleLogger());
}