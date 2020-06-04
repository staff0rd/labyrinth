using System.Threading.Tasks;
using Events;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading;

namespace Console
{
    public class ConsoleProgress : IProgress
    {
        private readonly ILogger<ConsoleProgress> _logger;
        Stopwatch _stopwatch;
        int _current;
        int _total;
        Timer _timer;
        

        public ConsoleProgress(ILogger<ConsoleProgress> logger)
        {
            _logger = logger;
            _timer = new Timer((_) => {
                if (_stopwatch != null)
                    Report();
            }, null, 0, 1000);
        }

        public void Report() {
            _logger.LogInformation($"Progress: {_current}/{_total}, {_stopwatch.Elapsed}");
        }

        public Task New()
        {
            if (_stopwatch != null)
                Report();
            
            _stopwatch = Stopwatch.StartNew();
            _current = 0;

            return Task.CompletedTask;
        }

        public async Task Set(int value)
        {
            if (_stopwatch == null)
                 await New();

            _logger.LogInformation($"Progress: {value}, {_stopwatch.Elapsed}");
        }

        public async Task Set(int current, int total)
        {
            _current = current;
            _total = total;

            if (_stopwatch == null)
                await New();

            if (current == total)
                Completed("Complete");
        }

        public void Completed(string message) {
            _stopwatch.Stop();
            _timer.Dispose();
            _logger.LogInformation($"{message}, {_stopwatch.Elapsed}");
        }
    }
}