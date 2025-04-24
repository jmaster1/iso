using System;
using Common.Bind;
using Common.Lang.Observable;
using Common.Lang.Collections;
using Timer = System.Timers.Timer;

namespace Common.TimeNS
{
    public class RunOnTime : BindableBean<Time>
    {
        private readonly ThreadSafeBuffer<Action> _actions = new();
        
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
            _actions.Flush(action => action());
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }
    }
}
