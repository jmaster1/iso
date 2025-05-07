using System;
using System.Collections.Generic;
using Common.Bind;
using Common.Lang.Collections;

namespace Common.TimeNS
{
    public class RunOnTime : BindableBean<Time>
    {
        private readonly ThreadSafeBuffer<Action> _actions = new();

        private readonly SortedSet<FrameAction> _frameActions = new();

        public Func<int>? FrameSupplier = null;
        
        protected override void OnBind()
        {
            Model.AddListener(OnTimeUpdate);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnTimeUpdate);
        }

        private void OnTimeUpdate(Time time)
        {
            _actions.Flush(action => action());
            var frame = FrameSupplier == null ? time.Frame : FrameSupplier?.Invoke();
            lock (_frameActions)
            {
                while (true)
                {
                    if (_frameActions.Count == 0 || _frameActions.Min.Frame > frame)
                        break;

                    var next = _frameActions.Min;
                    _frameActions.Remove(next);
                    Validate(next.Frame == frame);
                    next.Action();
                }
            }
        }

        public void AddAction(Action action)
        {
            _actions.Add(action);
        }
        
        public void AddAction(int frame, Action action)
        {
            lock (_frameActions)
            {
                _frameActions.Add(new FrameAction(frame, action));
            }
        }
    }

    internal class FrameAction : IComparable<FrameAction>
    {
        public int Frame { get; }

        public Action Action { get; }

        public FrameAction(int frame, Action action)
        {
            Frame = frame;
            Action = action;
        }

        public int CompareTo(FrameAction other)
        {
            return Frame - other.Frame;
        }
    }
}
