using System;
using Common.Api.Ads;
using Common.Api.Input;
using Common.Api.Local;
using Common.Bind;
using Common.ContextNS;
using Common.IO.Streams;
using Common.Lang;
using Common.Lang.Entity;
using Common.Lang.Observable;
using Common.TimeNS;
using Common.Unity.Api.Mono;
using Common.Unity.Api.Sprite;
using Common.Unity.Api.ViewPool;
using Common.Unity.Boot;
using Common.Unity.Util;
using Common.Util;
using Common.Util.Http;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Common.Unity.Bind
{
    /// <summary>
    /// MonoBehaviour extension with bind support
    /// </summary>
    /// <typeparam name="T">model type</typeparam>
    public class BindableMono<T> : BindableMonoRaw, IBindable<T>, IIdAware<T>, IHttpQueryProcessor
    {
        public static readonly ViewPoolApi ViewPoolApi  = Context.Get<ViewPoolApi>();
        
        public static readonly LocalApi LocalApi = Context.Get<LocalApi>();
        
        public static readonly SpriteApi SpriteApi = Context.Get<SpriteApi>();
        
        public static readonly MonoApi MonoApi = Context.Get<MonoApi>();
        
        public static readonly InputApi InputApi = Context.Get<InputApi>();
        
        private BindableForMono<T> bindable;

        private BindableForMono<T> Bindable => bindable ??= new BindableForMono<T>(this);

        public T Model => Bindable.GetModel();

        public BindableState GetBindableState()
        {
            return Bindable.GetBindableState();
        }

        public void Bind(T model)
        {
            Bindable.Bind(model);
        }
        
        internal sealed override void BindRaw(object model)
        {
            try
            {
                Bind((T) model);
            }
            catch (InvalidCastException ex)
            {
                LangHelper.Handle(ex, "View type={0}, expected model type={1}, actual model type={2}",
                    GetType().Name, typeof(T).Name, model.GetType().Name);
            }
        }

        public sealed override void Unbind()
        {
            Bindable.Unbind();
        }

        public T GetModel()
        {
            return Bindable.GetModel();
        }

        public Holder<T> GetModelHolder()
        {
            return Bindable.GetModelHolder();
        }

        public bool IsBound()
        {
            return Bindable.IsBound();
        }
        
        public bool IsBinding()
        {
            return Bindable.IsBinding();
        }

        public override void OnBind()
        {
        }
        
        public override void OnUnbind()
        {
        }
        
        /// <summary>
        /// bind given action to time interval
        /// </summary>
        protected void BindToTimer(Action updateAction, float interval = 1)
        {
            TimeTask task = null;
            void TimerUpdateAction()
            {
                updateAction();
                task.ScheduleAfterSec(interval);
            }
            task = Unicom.GameTimeTaskManager.CreateTask(TimerUpdateAction);
            task.ScheduleAfterSec(interval);
            //
            // create unbind action
            void UnbindAction()
            {
                task.Cancel();
            }
            bindable.AddUnbindAction(UnbindAction);
        }

        /// <summary>
        /// bind given action to holder updates, register for unbind
        /// </summary>
        protected void BindToHolder<TV>(Holder<TV> holder, 
            Action<Holder<TV>, TV, TV> updateAction)
        {
            LangHelper.Validate(holder != null);
            holder.AddListenerNotify(updateAction);
            //
            // create unbind action
            void UnbindAction()
            {
                holder.RemoveListener(updateAction);
            }
            bindable.AddUnbindAction(UnbindAction);
        }

        protected void BindToHolder<TV>(Holder<TV> holder, Action action, bool notify = true)
        {
            bindable.BindToHolder(holder, action, notify);
        }
        
        protected void BindToHolder<TV>(Holder<TV> holder, Action<TV> action, bool notify = true)
        {
            bindable.BindToHolder(holder, action, notify);
        }

        /// <summary>
        /// bind given bindable to holder value (un/bind each time holder value changes)
        /// </summary>
        public void BindBindableToHolder<TValue>(Holder<TValue> modelHolder, 
            IBindable<TValue> view) where TValue : class
        {
            if(view == null) return;
            void OnHolderChange(Holder<TValue> holder, TValue newVal, TValue oldVal)
            {
                if (newVal == null)
                {
                    view.Unbind();
                }
                else
                {
                    view.Bind(newVal);
                }
            }
            void UnbindAction()
            {
                modelHolder.RemoveListener(OnHolderChange);
                view.Unbind();
            }
            bindable.AddUnbindAction(UnbindAction);
            modelHolder.AddListenerNotify(OnHolderChange);
        }
        
        /// <summary>
        /// bind text update to holder value change
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="view"></param>
        /// <param name="transformer"></param>
        /// <typeparam name="TValue"></typeparam>
        protected void BindText<TValue>(Holder<TValue> holder, TMP_Text view, 
            Func<TValue, string> transformer = null)
        {
            
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                var value = transformer == null ? newVal.ToString() : transformer(newVal);
                view.text = value;
            }
            BindToHolder(holder, UpdateAction);
        }
        
        protected void BindText<TValue>(Holder<TValue> holder, TMP_Text view, 
            Func<string> transformer)
        {
            var wrapper = transformer == null ? (Func<TValue, string>)null : value => transformer();
            BindText(holder, view, wrapper);
        }
        
        protected void BindText(FloatHolder holder, TMP_Text view, string format)
        {
            BindText(holder, view, () => holder.Get().ToString(format));
        }
        
        protected void BindInputField<TValue>(Holder<TValue> holder, 
            TMP_InputField view, Func<string> transformer = null)
        {
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                var value = transformer == null ? newVal.ToString() : transformer();
                view.text = value;
            }
            BindToHolder(holder, UpdateAction);
        }
        
        protected void BindImage(Enum model, Image view, string suffix = null)
        {
            if(view == null) return;
            var sprite = SpriteApi.GetSprite(model, suffix);
            view.sprite = sprite;
            bindable.AddUnbindAction(() => view.sprite = null);
        }
        
        protected void BindImage(AbstractEntityIdString model, Image view)
        {
            if(view == null) return;
            var sprite = SpriteApi.GetSprite(model);
            view.sprite = sprite;
            bindable.AddUnbindAction(() => view.sprite = null);
        }

        protected void BindSlider<TValue>(Holder<TValue> holder, Func<float> valueGetter, Slider view, 
            float min = 0, float max = 1, Action<float> valueSetter = null)
        {
            view.minValue = min;
            view.maxValue = max;
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                view.value = valueGetter();
            }
            BindToHolder(holder, UpdateAction);
            //
            // listen to slider changes to invoke setter
            if (valueSetter != null)
            {
                void OnSliderValueChanged(float val)
                {
                    valueSetter(val);
                }
                
                view.onValueChanged.AddListener(OnSliderValueChanged);
                bindable.AddUnbindAction(() => { view.onValueChanged.RemoveListener(OnSliderValueChanged);});
            }
        }

        protected void BindSlider(FloatHolder holder, Slider view, float min = 0, float max = 1)
        {
            BindSlider(holder, holder.Get, view, min, max, val => holder.Set(val));
        }
        
        protected void BindSlider(FloatHolder holder, Slider view, float[] minMax)
        {
            BindSlider(holder, view, minMax[0], minMax[1]);
        }

        /// <summary>
        /// bind component activity to specified holder updates using activeFunc to eval component active state
        /// </summary>
        protected void BindActive<TValue>(Holder<TValue> holder, GameObject go, Func<bool> activeFunc = null, bool invert = false)
        {
            if (activeFunc == null) activeFunc = () => !holder.IsNull();
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                var active = activeFunc();
                active = invert ? !active : active;
                go.SetActive(active);
            }
            BindToHolder(holder, UpdateAction);
        }
        
        protected void BindActive<TValue>(Holder<TValue> holder, Component comp, Func<bool> activeFunc = null, bool invert = false)
        {
            BindActive(holder, comp.gameObject, activeFunc, invert);
        }
        
        protected void BindActive(BoolHolder holder, GameObject go, bool invert = false)
        {
            void UpdateAction(Holder<bool> h, bool newVal, bool oldVal)
            {
                var active = invert ? !newVal : newVal;
                go.SetActive(active);
            }
            BindToHolder(holder, UpdateAction);
        }
        
        public void BindActive(BoolHolder holder, Component comp, bool invert = false)
        {
            BindActive(holder, comp.gameObject, invert);
        }

        public void BindActiveInvert<TValue>(Holder<TValue> holder, Component comp, Func<bool> activeFunc = null)
        {
            BindActive(holder, comp, activeFunc, true);
        }

        public void BindActiveInvert(BoolHolder holder, Component comp)
        {
            BindActive(holder, comp, true);
        }

        public void BindActiveInvert(BoolHolder holder, GameObject go = null)
        {
            BindActive(holder, go, true);
        }
        
        /// <summary>
        /// bind component interactable state to specified holder
        /// </summary>
        public void BindInteractable(BoolHolder holder, Selectable selectable, bool invert = false)
        {
            void UpdateAction(Holder<bool> h, bool newVal, bool oldVal)
            {
                var interactable = holder.Get();
                selectable.interactable = invert ? !interactable : interactable;
            }
            BindToHolder(holder, UpdateAction);
        }
        
        /// <summary>
        /// bind component interactable state to specified holder
        /// </summary>
        public void BindInteractable<TValue>(Holder<TValue> holder, Selectable selectable, Func<bool> func)
        {
            void UpdateAction(Holder<TValue> h, TValue newVal, TValue oldVal)
            {
                var interactable = func();
                selectable.interactable = interactable;
            }
            BindToHolder(holder, UpdateAction);
        }
        
        /// <summary>
        /// install button click handler that will be unregistered on unbind
        /// </summary>
        public void BindClick(Button button, UnityAction action)
        {
            LangHelper.Validate(button != null, "Null button passed to BindClick()");
            void OnClick()
            {
                MonoApi.ButtonClicked(button);
                action.Invoke();
            }
            button.onClick.AddListener(OnClick);
            void UnbindAction()
            {
                button.onClick.RemoveListener(OnClick);
            }
            Bindable.AddUnbindAction(UnbindAction);
        }
        
        /// <summary>
        /// install toggle click handler that will be unregistered on unbind
        /// </summary>
        public void BindClick(Toggle toggle, UnityAction<bool> action)
        {
            toggle.onValueChanged.AddListener(action);
            void UnbindAction()
            {
                toggle.onValueChanged.RemoveListener(action);
            }
            Bindable.AddUnbindAction(UnbindAction);
        }

        public void BindToggle(Toggle toggle, UnityAction<bool> action)
        {
            void OnToggle(bool value)
            {
                action.Invoke(value);
            }
            toggle.onValueChanged.AddListener(OnToggle);

            void UnbindAction()
            {
                toggle.onValueChanged.RemoveListener(OnToggle);
            }
            Bindable.AddUnbindAction(UnbindAction);
        }
        
        public void BindToggle(BoolHolder holder, Toggle toggle, UnityAction<bool> action)
        {
            BindToHolder(holder, b => { toggle.isOn = b;});
            if(action != null) BindClick(toggle, action);
        }

        public void BindBindable<TM>(TM model, IBindable<TM> view)
        {
            LangHelper.Validate(view != null, "Null view");
            Bindable.BindBindable(model, view);
        }
        
        /// <summary>
        /// register events listener (unregister OnUnbind)
        /// </summary>
        protected void BindEvents<TEvent, TPayload>(Events<TEvent, TPayload> events, Action<TEvent, TPayload> action) where TEvent : Enum
        {
            Bindable.BindEvents(events, action);
        }
        
        protected void BindObsList<TE>(ObsList<TE> list, Action<TE, int> onAdd, Action<TE, int> onRemove, Action onClear)
        {
            void OnEvent(ObsListEvent evt, ObsListEventData<TE> payload)
            {
                switch (evt)
                {
                    case ObsListEvent.AddAfter:
                        onAdd(payload.Element, payload.Index);
                        break;
                    case ObsListEvent.RemoveBefore:
                        onRemove(payload.Element, payload.Index);
                        break;
                    case ObsListEvent.ClearBefore:
                        onClear();
                        break;
                    case ObsListEvent.AddBefore:
                    case ObsListEvent.RemoveAfter:
                    case ObsListEvent.ClearAfter:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(evt), evt, null);
                }
            }
            list.events.AddListener(OnEvent);
            Bindable.AddUnbindAction(() => list.events.RemoveListener(OnEvent));
        }
        
        protected void BindModelEvents<TEvent>(Events<TEvent, T> events, Action<TEvent> action) where TEvent : Enum
        {
            Bindable.BindEvents(events, (evt, model) =>
            {
                if (LangHelper.Equals(model, Model))
                {
                    action(evt);
                }
            });
        }
        
        /// <summary>
        /// bind ui to AdsPlacement
        /// </summary>
        /// <param name="placement"></param>
        /// <param name="button">a button which interactable state will be bound to placement.Available,
        /// button click will trigger placement.ShowRewardedVideo</param>
        /// <param name="container">optional container which activity should be bound to placement.Available</param>
        public void BindAdsPlacement(AdsPlacement placement, Button button, 
            MonoBehaviour container = null, bool buttonInteractable = true)
        {
            if(buttonInteractable) BindInteractable(placement.Available, button);
            else BindActive(placement.Available, button);
            if(container) BindActive(placement.Available, container);
            BindClick(button, () => placement.ShowAds());
        }

        public override object GetModelRaw()
        {
            return Bindable.GetModel();
        }

        public static Color ToColor(uint rgba)
        {
            return UnityHelper.RgbaToColor(rgba);
        }
        
        public virtual void OnHttpRequest(HttpQuery request)
        {
        }

        public virtual void OnHttpResponse(HttpQuery query, HtmlWriter html)
        {
        }

        public T GetId()
        {
            return Model;
        }
    }
}
