using System;
using Common.Bind;
using Common.Lang.Observable;
using Common.Lang.Collections;
using Timer = System.Timers.Timer;

namespace Common.TimeNS
{
    /// <summary>
    /// observable time that updated externally or by binding to another time
    /// </summary>
    public class Time : BindableBean<Time>
    {
        /// <summary>
        /// time listeners
        /// </summary>
        private readonly Listeners<Action<Time>> _listeners = new();

        /// <summary>
        /// current value (ms)
        /// </summary>
        public DateTime Value { get; protected set; }

        /// <summary>
        /// update delta
        /// </summary>
        public TimeSpan Delta { get; protected set; }

        /// <summary>
        /// retrieve current time value in seconds
        /// </summary>
        public float ValueSeconds => Value.Ticks / 10000000f;
        
        /// <summary>
        /// offset to add to system time
        /// </summary>
        public TimeSpan Offset = TimeSpan.Zero;
        
        /// <summary>
        /// number of Update() calls made so far
        /// </summary>
        public int Frame { get; private set; }

        private Timer? _timer;
        
        private readonly ThreadSafeBuffer<Action> _runOnUpdate = new();

        public void AddListener(Action<Time> e)
        {
            _listeners.Add(e);
        }
        
        public void AddListenerSafe(Action<Time> e)
        {
            _listeners.AddSafe(e);
        }

        public void RemoveListener(Action<Time> e)
        {
            _listeners.Remove(e);
        }
        
        public bool ContainsListener(Action<Time> e)
        {
            return _listeners.Contains(e);
        }

        protected override void OnBind()
        {
            Model.AddListener(OnTimeUpdate);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnTimeUpdate);
        }

        private void OnTimeUpdate(Time parent)
        {
            Update();
        }
        
        public virtual void Update()
        {
            Update(Model.Delta);
        }
        
        public void Update(TimeSpan delta)
        {
            Frame++;
            Delta = delta;
            Value += delta;
            _runOnUpdate.Flush(action => action());
            Notify();
        }
        
        public void UpdateSec(float deltaSec)
        {
            Update(TimeSpan.FromSeconds(deltaSec));
        }

        protected void Notify()
        {
            for(int i = 0, n = _listeners.Begin(); i < n; i++)
            {
                var listener = _listeners.Get(i);
                listener(this);
            }
            _listeners.End();
        }

        public void StartTimer(TimeSpan delta)
        {
            if (IsTimerRunning())
            {
                StopTimer();
            }
            _timer = new Timer(delta.TotalMilliseconds);
            _timer.Elapsed += (sender, e) =>
            {
                Update(delta);
            };
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void StopTimer()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
        }

        public bool IsTimerRunning()
        {
            return _timer is { Enabled: true };
        }
        

        public void RunOnUpdate(Action action)
        {
            _runOnUpdate.Add(action);
        }
    }
}
