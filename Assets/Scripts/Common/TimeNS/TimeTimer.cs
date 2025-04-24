using System;
using Timer = System.Timers.Timer;

namespace Common.TimeNS
{
    public class TimeTimer
    {
        private Timer? _timer;

        public void Start(Time time, TimeSpan delta)
        {
            if (IsRunning())
            {
                Stop();
            }
            _timer = new Timer(delta.TotalMilliseconds);
            _timer.Elapsed += (sender, e) =>
            {
                time.Update(delta);
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        public bool IsRunning()
        {
            return _timer is { Enabled: true };
        }
    }
}
