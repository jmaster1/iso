using System;
using System.Collections.Generic;
using Common.Util;

namespace Common.Lang.Observable
{
    /// <summary>
    /// observable value container, listeners notified after update and receive 3 parameters:
    /// Holder (this), newValue (T), oldValue (T)
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public class Holder<T> : HolderRaw
    {
        private readonly Listeners<Action<Holder<T>, T, T>> listeners = new Listeners<Action<Holder<T>, T, T>>();
            
        protected T value;

        public T Value
        {
            get => value;
            set
            {
                Set(value);
            }
        }

        public Holder()
        {
        }
        
        public Holder(T value)
        {
            this.value = value;
        }

        public bool Updating { get; private set; }

        /// <summary>
        /// list of actions to invoke post update/notify listeners,
        /// used to execute logic that might change holder value on it's value change to prevent
        /// mutation in notification 
        /// </summary>
        private List<Action> postUpdateActions;

        /// <summary>
        /// check if current value equals specified
        /// </summary>
        public bool Is(T val)
        {
            if (val == null && value == null) return true;
            return value != null && value.Equals(val);
        }
        
        /// <summary>
        /// check if current value is null
        /// </summary>
        public bool IsNull()
        {
            return value == null;
        }
        
        /// <summary>
        /// current value retrieval
        /// </summary>
        public virtual T Get()
        {
            return value;
        }
        
        /// <summary>
        /// update holder value
        /// </summary>
        /// <param name="v"></param>
        /// <returns>new value</returns>
        public T Set(T v)
        {
            if (Updating)
            {
                throw new InvalidOperationException();
            }
            LangHelper.Validate(!listeners.Started, "Mutate in notification prohibited!");
            var oldVal = Get();
            if (Equals(v, oldVal))
            {
                return v;
            }
            Updating = true;
            SetInternal(v);
            var newVal = Get();
            var n = listeners.Begin();
            try
            {
                for (var i = 0; i < n; i++)
                {
                    var listener = listeners.Get(i);
                    listener(this, newVal, oldVal);
                }
            }
            finally
            {
                listeners.End();
                Updating = false;
            }

            //
            // invoke post-update actions
            if (postUpdateActions != null && postUpdateActions.Count > 0)
            {
                var arr = postUpdateActions.ToArray();
                postUpdateActions.Clear();
                for (var i = 0; i < arr.Length; i++)
                {
                    arr[i].Invoke();
                }
            }

            return newVal;
        }

        /// <summary>
        /// internal setter
        /// </summary>
        protected virtual void SetInternal(T v)
        {
            value = v;
        }

        /// <summary>
        /// register listener
        /// </summary>
        /// <param name="e">action with 3 args: holder, newValue, oldValue</param>
        /// <param name="notify">flag to notify listener about initial value</param>
        public void AddListener(Action<Holder<T>, T, T> e, bool notify = false)
        {
            listeners.Add(e);
            if (!notify) return;
            var val = Get();
            e(this, val, val);
        }

        public void AddListenerNotify(Action<Holder<T>, T, T> e)
        {
            AddListener(e, true);
        }
        
        /// <summary>
        /// unregister listener
        /// </summary>
        public void RemoveListener(Action<Holder<T>, T, T> e)
        {
            listeners.Remove(e);
        }

        /// <summary>
        /// nullify value (or default for value type)
        /// </summary>
        public T SetDefault()
        {
            return Set(default);
        }

        public override void Clear()
        {
            SetDefault();
        }
        
        public override string ToString()
        {
            return Get().ToString();
        }

        public override object GetRaw()
        {
            return Get();
        }

        public override void SetRaw(object val)
        {
            Set((T) val);
        }

        public override Type GetValueType()
        {
            return typeof(T);
        }

        /// <summary>
        /// register listener to this holder that will invoke onComplete
        /// when holder value equals specified one
        /// </summary>
        /// <param name="val">expected holder value to await</param>
        /// <param name="onComplete">action to invoke</param>
        /// <param name="cancelOnComplete">if true, then remove listener on match</param>
        /// <returns>remove action (invoke it to remove listener)</returns>
        public Action AwaitValue(T val, Action onComplete, bool cancelOnComplete = true)
        {
            void Listener(Holder<T> holder, T newVal, T oldVal)
            {
                if (!LangHelper.Equals(val, newVal)) return;
                if (cancelOnComplete)
                {
                    RemoveListener(Listener);
                }

                InvokePostUpdate(onComplete);
            }

            void Cancel()
            {
                RemoveListener(Listener);
            }

            AddListenerNotify(Listener);
            return Cancel;
        }

        /// <summary>
        /// invoke action once after value update/listeners notification
        /// </summary>
        /// <param name="action"></param>
        public void InvokePostUpdate(Action action)
        {
            if (postUpdateActions == null)
            {
                postUpdateActions = new List<Action>();
            }
            postUpdateActions.Add(action);
        }
    }
}
