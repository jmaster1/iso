using System;
using Common.Bind;
using Common.Lang.Collections;

namespace Common.Lang.Observable
{
    /// <summary>
    /// Bindable extension that keeps list of views in sync with list of model elements
    /// </summary>
    /// <typeparam name="TM">element type of model</typeparam>
    /// <typeparam name="TV">view type for model element</typeparam>
    public class ObsListAdapter<TM, TV> : BindableBean<ObsList<TM>>
    {
        /// <summary>
        /// model > view map
        /// </summary>
        public ObsList<TV> Views = new();

        /// <summary>
        /// factory for creating views, this might return null
        /// </summary>
        public Func<TM, int, TV> CreateView;
        
        /// <summary>
        /// will be invoked once for each client element after added
        /// </summary>
        public Action<TM, TV> PostAdd;
        
        /// <summary>
        /// action for desposing view objects
        /// </summary>
        public Action<TV> DisposeView;

        private readonly Map<TM, TV> modelToView = new();

        protected override void OnBind()
        {
            Model.AddListenerNotify(OnListChange);
        }

        protected override void OnUnbind()
        {
            Model.RemoveListener(OnListChange);
            ClearViews();
        }
        
        private void OnListChange(ObsListEvent type, ObsListEventData<TM> data)
        {
            switch (type)
            {
                case ObsListEvent.AddAfter:
                    OnAdd(data.Element, data.Index);
                    break;
                case ObsListEvent.RemoveAfter:
                    OnRemove(data.Element, data.Index);
                    break;
                case ObsListEvent.ClearAfter:
                    ClearViews();
                    break;
            }
        }
        
        protected virtual void ClearViews()
        {
            foreach(var e in Views)
            {
                DisposeView(e);
            }
            Views.Clear();
        }

        protected virtual void OnRemove(TM element, int index)
        {
            var view = Views.RemoveAtGet(index);
            modelToView[element] = default;
            DisposeView(view);       
        }

        protected virtual void OnAdd(TM element, int index)
        {
            var view = CreateView(element, index);
            modelToView[element] = view;
            if (view == null) return;
            Views.Add(view);
            PostAdd?.Invoke(element, view);
        }

        public TV GetView(TM model)
        {
            return modelToView[model];
        }

        /// <summary>
        /// recreate view for given model
        /// </summary>
        public void Recreate(TM el)
        {
            var view = GetView(el);
            var index = Model.IndexOf(el);
            if (view != null)
            {
                OnRemove(el, index);
            }
            OnAdd(el, index);
        }
    }
}