using System;
using Timer = System.Timers.Timer;

namespace Common.TimeNS
{
    public class TimeTimer
    {
        private readonly object _lock = new();
        private Timer? _timer;

        public void Start(TimeSpan delta, Action<TimeSpan> update)
        {
            lock (_lock)
            {
                StopInternal();
                _timer = new Timer(delta.TotalMilliseconds);
                _timer.Elapsed += (_, _) =>
                {
                    lock (_lock)
                    {
                        if (_timer is { Enabled: true })
                        {
                            update(delta);
                        }
                    }
                };
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }
        }

        public void Start(TimeSpan delta, Time time)
        {
            Start(delta, span => time.Update(delta));
        }

        public void Stop()
        {
            lock (_lock)
            {
                StopInternal();
            }
        }

        public bool IsRunning()
        {
            lock (_lock)
            {
                return _timer is { Enabled: true };
            }
        }

        private void StopInternal()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }
    }
}
