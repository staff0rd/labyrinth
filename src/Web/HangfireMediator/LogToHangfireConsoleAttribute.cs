using Hangfire.Server;
using Hangfire.Common;
using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public class LogToHangfireConsoleAttribute : JobFilterAttribute, IServerFilter
{
    private IDisposable _subscription;

    public void OnPerforming(PerformingContext filterContext)
    {
        _subscription = HangfireConsoleLogger.InContext(filterContext);
    }
    public void OnPerformed(PerformedContext filterContext)
    {
        _subscription?.Dispose();
    }
}