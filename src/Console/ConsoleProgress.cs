using System;
using System.Threading.Tasks;
using Events;
using ShellProgressBar;

namespace Console
{
    public class ConsoleProgress : IProgress
    {
        ProgressBar _progress;
        int _ticks = 100;

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

        public async Task Set(int value)
        {
            if (_progress == null)
                await New();

            _progress.Tick(value);
        }

        public async Task Set(int current, int total)
        {
            int progress = current * 100 / total;
            await Set(progress);
        }
    }
}