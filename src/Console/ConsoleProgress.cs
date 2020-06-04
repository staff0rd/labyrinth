using System;
using System.Threading.Tasks;
using Events;
using ShellProgressBar;
using Microsoft.Extensions.Logging;

namespace Console
{
    public class ConsoleProgress : IProgress
    {
        private readonly ILogger<ConsoleProgress> _logger;
        ProgressBar _progress;
        int _ticks = 100;

        public ConsoleProgress(ILogger<ConsoleProgress> logger)
        {
            _logger = logger;
        }

        public Task New()
        {
            if (_progress != null)
                _progress.Dispose();

            var options = new ProgressBarOptions
            {
                ProgressCharacter = '─',
                ProgressBarOnBottom = true
            };
            _progress = new ProgressBar(_ticks, "Initial message", options);

            return Task.CompletedTask;
        }

        public Task Set(int value)
        {
            // if (_progress == null)
            //     await New();

            _logger.LogInformation($"Progress: {value}");
            return Task.CompletedTask;
        }

        public Task Set(int current, int total)
        {
            int progress = current * 100 / total;
            _logger.LogInformation($"Progress: {current}/{total}");
            return Task.CompletedTask;
        }
    }
}