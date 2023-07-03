using System;
using System.Collections.Generic;
using Common.Bind;

namespace Common.Lang.Observable
{
    /// <summary>
    /// responsible for keeping internal list in sync with model 
    /// </summary>
    public class ObsListSyncAdapter<T, S> : BindableBean<ObsList<T>>
    {
        /// <summary>
        /// list being synchronized
        /// </summary>
        public List<S> List = new List<S>();

        /// <summary>
        /// this MUST be injected externally to create sync list elements
        /// </summary>
        public Func<T, S> Factory;
        
        /// <summary>
        /// this should be injected externally in order to dispose sync list elements
        /// </summary>
        public Action<T, S> Destructor;
        
        protected override void OnBind()
        {
            Model.AddListenerNotify(OnListChange);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnListChange);
            Clear();
        }

        private void OnListChange(ObsListEvent type, ObsListEventData<T> data)
        {
            var t = data.Element;
            var i = data.Index;
            switch (type)
            {
                case ObsListEvent.AddAfter:
                    UnityEngine.Debug.Assert(i == List.Count);
                    S s = Factory(t);
                    List.Add(s);
                    break;
                case ObsListEvent.RemoveBefore:
                    s = List[i];
                    if (Destructor != null)
                    {
                        Destructor(t, s);
                    }
                    List.RemoveAt(i);
                    break;
                case ObsListEvent.ClearBefore:
                    Clear();
                    break;
            }
        }

        public override void Clear()
        {
            if (Destructor != null)
            {
                for (int i = Model.Count - 1; i >= 0; i--)
                {
                    T t = Model[i];
                    S s = List[i];
                    List.RemoveAt(i);
                    Destructor(t, s);
                }
            }
            else
            {
                List.Clear();
            }
        }
    }
}