using System;
using System.Reflection;
using Common.Unity.Util;
using Common.Unity.View;
using Common.Util;
using Common.Util.Log;
using Common.View;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.Bind
{
    public abstract class BindableMonoRaw : MonoBehaviour, IView
    {
        private LogWrapper log;
        
        public LogWrapper Log => log ?? (log = new LogWrapper(GetType()));
        
        /// <summary>
        /// animator function
        /// </summary>
        public Action<GameObject, bool, Action> Animator = DefaultAnimator.PlayAnimation;

        public abstract object GetModelRaw();
        
        internal abstract void BindRaw(object model);
        
        public abstract void OnBind();
        
        public abstract void OnUnbind();
        
        public abstract void Unbind();

        private const BindingFlags FieldBindingFlags = BindingFlags.Instance |
                                                       BindingFlags.NonPublic |
                                                       BindingFlags.Public;
        /// <summary>
        /// default button resolver, used to handle back/sec actions
        /// </summary>
        public virtual Button FindDefaultButton()
        {
            var type = GetType();
            var fields = type.GetFields(FieldBindingFlags);
            if (!fields.IsNotEmpty()) return null;
            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<DefaultButtonAttribute>();
                if (attr == null) continue;
                var value = field.GetValue(this);
                var button = value as Button;
                LangHelper.Validate(button != null, 
                    "Expected button for field {0}, got: {1}",
                    field, value);
                return button;
            }
            return null;
        }
        
        /// <summary>
        /// back action handler, this implementation resolves default button,
        /// then invokes onClick for it (if exists and active and interactable),
        /// subclasses may override this
        /// </summary>
        /// <returns>true if request processed, otherwise false</returns>
        public virtual bool OnBack()
        {
            var button = FindDefaultButton();
            if (button == null || !button.IsActive() || !button.IsInteractable())
            {
                return false;
            }

            button.onClick.Invoke();
            return true;
        }

        public virtual bool PlayAnimation(bool show, Action onComplete)
        {
            if (Animator == null)
            {
                return false;
            }
            Animator(gameObject, show, onComplete);
            return true;
        }

        public ViewInstance ViewInstance { get; set; }

        /// <summary>
        /// re-bind to current model invoking action in-between 
        /// </summary>
        /// <param name="action"></param>
        public void Rebind(Action action)
        {
            var model = GetModelRaw();
            Unbind();
            action?.Invoke();
            BindRaw(model);
        }
        
        /// <summary>
        /// should be called to animate components
        /// </summary>
        /// <param name="show"></param>
        /// <param name="onComplete"></param>
        /// <returns></returns>
        protected UIAnimator Animate(bool show, Action onComplete)
        {
            return new UIAnimator(this, show, onComplete);
        }
    }
}