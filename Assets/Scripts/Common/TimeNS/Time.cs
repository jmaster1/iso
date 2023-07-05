using System;
using Common.Bind;
using Common.Lang.Observable;

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
        private readonly Listeners<Action<Time>> listeners = new Listeners<Action<Time>>();

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

        public void AddListener(Action<Time> e)
        {
            listeners.Add(e);
        }
        
        public void AddListenerSafe(Action<Time> e)
        {
            listeners.AddSafe(e);
        }

        public void RemoveListener(Action<Time> e)
        {
            listeners.Remove(e);
        }
        
        public bool ContainsListener(Action<Time> e)
        {
            return listeners.Contains(e);
        }

        protected override void OnBind()
        {
            Model.AddListener(OnTimeUpdate);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnTimeUpdate);
        }

        protected void OnTimeUpdate(Time parent)
        {
            Update();
        }
        
        public virtual void Update()
        {
            Update(Model.Delta);
        }
        
        public void Update(TimeSpan delta)
        {
            Delta = delta;
            Value += delta;
            Notify();
        }
        
        public void UpdateSec(float deltaSec)
        {
            Update(TimeSpan.FromSeconds(deltaSec));
        }

        protected void Notify()
        {
            for(int i = 0, n = listeners.Begin(); i < n; i++)
            {
                var listener = listeners.Get(i);
                listener(this);
            }
            listeners.End();
        }


    }
}