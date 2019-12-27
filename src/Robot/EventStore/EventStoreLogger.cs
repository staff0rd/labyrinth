using System;
using EventStore.ClientAPI;
using Microsoft.Extensions.Logging;

namespace Robot
{
    public class EventStoreLogger : EventStore.ClientAPI.ILogger
    {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        public EventStoreLogger(Microsoft.Extensions.Logging.ILogger logger)
        {
            _logger = logger;
        }

        public void Debug(string format, params object[] args)
        {
            _logger.LogDebug(format, args);
        }

        public void Debug(Exception ex, string format, params object[] args)
        {
            _logger.LogDebug(ex, format, args);
        }

        public void Error(string format, params object[] args)
        {
            _logger.LogError(format, args);
        }

        public void Error(Exception ex, string format, params object[] args)
        {
            _logger.LogError(ex, format, args);
        }

        public void Info(string format, params object[] args)
        {
            _logger.LogInformation(format, args);
        }

        public void Info(Exception ex, string format, params object[] args)
        {
            _logger.LogInformation(ex, format, args);
        }
    }
}