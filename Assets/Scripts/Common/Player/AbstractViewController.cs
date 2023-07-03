using System;
using System.Threading.Tasks;
using Common.Bind;
using Common.View;

namespace Common.Player
{
    /// <summary>
    /// base class for feature-based view controller.
    /// </summary>
    /// <typeparam name="T">parent feature type</typeparam>
    public abstract class AbstractViewController<T> : BindableBean<T>, IViewController
    {
        public abstract string ViewId { get; }
        
        public abstract string LayerId { get; }

        internal ViewManager ViewManager;
        
        internal ViewInstance ViewInstance;
        
        protected virtual string ToViewId(Enum viewId)
        {
            return "UI/" + viewId.ToString().Replace('_', '/');
        }

        public Task Show()
        {
            Validate(IsBound());
            Validate(ViewManager != null, "ViewManager not set");
            ViewInstance ??= ViewManager.Create(ViewId, this).SetLayer(LayerId);
            OnShow();
            return ViewInstance.Show();
        }

        /// <summary>
        /// bind and show at once, useful for one-show views
        /// </summary>
        public Task Show(T model, ViewManager viewManager)
        {
            ViewManager = viewManager;
            Bind(model);
            return Show();
        }
        
        /// <summary>
        /// subj to override, invoked on show
        /// </summary>
        protected virtual void OnShow()
        {
        }

        public Task Hide()
        {
            Validate(ViewInstance != null);
            OnHide();
            return ViewInstance.Hide();
        }

        /// <summary>
        /// subj to override, invoked on hide
        /// </summary>
        protected virtual void OnHide()
        {
        }

        public void OnClose()
        {
            Hide();
        }
    }
}