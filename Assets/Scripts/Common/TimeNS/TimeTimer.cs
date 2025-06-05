using System;
using Timer = System.Timers.Timer;

namespace Common.TimeNS
{
    public class TimeTimer
    {
        private readonly object _lock = new();
        private Timer? _timer;

        public void Start(Time time, TimeSpan delta)
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
                            time.Update(delta);
                        }
                    }
                };
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }
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
