using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Utils
{
    public class Timer
    {
        public int DelaySeconds { get; }
        public event EventHandler TimeElapsed;

        private bool started;

        public Timer(int delaySeconds)
        {
            DelaySeconds = delaySeconds;
        }

        public async Task StartAsync(CancellationToken token = default)
        {
            if (started)
            {
                return;
            }

            started = true;

            while (!token.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(DelaySeconds), token);
                TimeElapsed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
