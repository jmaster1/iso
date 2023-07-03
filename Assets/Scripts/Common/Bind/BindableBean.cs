using System;
using System.Collections.Generic;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.Util;

namespace Common.Bind
{
    /// <summary>
    /// IBindable generic implementation
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BindableBean<T> : GenericBean, IBindable<T>
    {
        /// <summary>
        /// current state of bindable
        /// </summary>
        private BindableState bindableState = BindableState.Unbound;

        /// <summary>
        /// model reference
        /// </summary>
        public T Model;
        
        /// <summary>
        /// model observable container
        /// </summary>
        public readonly Holder<T> ModelHolder = new Holder<T>();
        
        /// <summary>
        /// actions to invoke on unbind
        /// </summary>
        public readonly List<Action> unbindActions = new List<Action>();

        public BindableState GetBindableState()
        {
            return bindableState;
        }

        public void Bind(T model)
        {
            LangHelper.Validate(model != null);
            LangHelper.Validate(!bindableState.IsTemporal());
            Unbind();
            bindableState = BindableState.Binding;
            Model = model;
            OnBind();
            ModelHolder.Set(model);
            bindableState = BindableState.Bound;
        }

        public void Unbind()
        {
            LangHelper.Validate(!bindableState.IsTemporal());
            if (!IsBound())
            {
                return;
            }
            bindableState = BindableState.Unbinding;
            //
            // invoke unbind actions
            foreach (var e in unbindActions)
            {
                e();
            }
            unbindActions.Clear();
            ModelHolder.SetDefault();
            OnUnbind();
            Model = default;
            bindableState = BindableState.Unbound;
        }

        public T GetModel()
        {
            return Model;
        }

        public Holder<T> GetModelHolder()
        {
            return ModelHolder;
        }

        public bool IsBound()
        {
            return bindableState == BindableState.Bound;
        }
        
        public bool IsBinding()
        {
            return bindableState == BindableState.Binding;
        }

        public bool IsBoundOrBinding => IsBound() || IsBinding();

        /// <summary>
        /// called on bind after model assignment, subj for override
        /// </summary>
        protected virtual void OnBind()
        {
        }

        /// <summary>
        /// called on unbind before model unassignment, subj for override
        /// </summary>
        protected virtual void OnUnbind()
        {
        }

        public void AddUnbindAction(Action action)
        {
            unbindActions.Add(action);
        }
        
        /// <summary>
        /// bind specified bindable to given model, register for unbind
        /// </summary>
        public void BindBindable<TModel>(TModel model, IBindable<TModel> view)
        {
            Validate(view != null);
            if (model != null)
            {
                view.Bind(model);
                AddUnbindAction(() => view.Unbind());
            }
            else
            {
                view.Unbind();
            }
        }
        
        /// <summary>
        /// bind action to holder, will invoke initially and each time holder value changed
        /// </summary>
        public void BindToHolder<TValue>(Holder<TValue> holder, Action<TValue> action, bool notify = true)
        {
            Assert(holder != null);
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                action(newVal);
            }
            holder.AddListener(UpdateAction, notify);
            AddUnbindAction(() => holder.RemoveListener(UpdateAction));
        }
        
        public void BindToHolder<TValue>(Holder<TValue> holder, Action action, bool notify = true)
        {
            Assert(holder != null);
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                action();
            }
            holder.AddListener(UpdateAction, notify);
            AddUnbindAction(() => holder.RemoveListener(UpdateAction));
        }
        
        /// <summary>
        /// register events listener (unregister OnUnbind)
        /// </summary>
        public void BindEvents<TE, TP>(Events<TE, TP> events, Action<TE, TP> action) where TE : Enum
        {
            events.AddListener(action);
            AddUnbindAction(() => events.RemoveListener(action));
        }
        
        public void BindEventsNotify<TX>(ObsList<TX> list, 
            Action<ObsListEvent, ObsListEventData<TX>> listener)
        {
            list.AddListenerNotify(listener);
            AddUnbindAction(() => list.RemoveListener(listener));
        }

        protected void AssertBound()
        {
            Assert(IsBound() || IsBinding());
        }
        
        public static void BindAll<TModel>(TModel model, params IBindable<TModel>[] bindables)
        {
            foreach (var e in bindables)
            {
                e.Bind(model);
            }
        }
    }
}
