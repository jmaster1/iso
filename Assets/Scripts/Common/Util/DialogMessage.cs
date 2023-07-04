using System;
using System.Collections.Generic;
using Common.ContextNS;
using Common.Lang;
using Common.Lang.Collections;
using Common.View;

namespace Common.Util
{
    /// <summary>
    /// model for generic dialog with title/message/buttons (actions),
    /// it is important to initialize Launch action that responsible to attach ui to created message
    /// sample usage:
    /// new DialogMessage()
    /// {
    /// Title = "title",
    /// Message = "message"
    /// }
    /// .AddAction("Ok", () =&gt; {})
    /// .AddAction("Cancel", () =&gt; {})
    /// .Show();
    /// </summary>
    public class DialogMessage
    {
        public static ViewManager ViewManager = Context.Get<ViewManager>();
        
        /// <summary>
        /// dialog title
        /// </summary>
        public string Title;
        
        /// <summary>
        /// dialog message
        /// </summary>
        public string Message;

        /// <summary>
        /// dialog actions (buttons), the key is button text
        /// </summary>
        private readonly Map<string, Action> actions = new Map<string, Action>();

        private ViewInstance viewInstance;
        
        private string DefaultAction;

        public IEnumerable<string> ActionNames => actions.Keys;

        /// <summary>
        /// this will be invoked on dialog close (if set)
        /// </summary>
        public event Action OnClose;

        /// <summary>
        /// add dialog action (button)
        /// </summary>
        /// <param name="name">unique action name (button label)</param>
        /// <param name="action">an action to invoke</param>
        /// <param name="defaultAction">if true, action will be triggered upon back/esc key press</param>
        /// <returns></returns>
        public DialogMessage AddAction(string name, Action action = null, bool defaultAction = false)
        {
            actions[name] = action;
            if (defaultAction)
            {
                DefaultAction = name;
            }
            return this;
        }
        
        public DialogMessage AddActionOk(Action action = null, bool defaultAction = true)
        {
            return AddAction("Ok", action, defaultAction);
        }

        /// <summary>
        /// show dialog
        /// </summary>
        public void Show()
        {
            viewInstance = ViewManager.Create("UI/Common/DialogMessageView", this)
                .SetLayer(ViewManager.LayerDebug);
            viewInstance.Show();
        }

        /// <summary>
        /// should be invoked by view on action button click
        /// </summary>
        public void OnAction(string name)
        {
            var action = actions[name];
            action?.Invoke();
            OnClose?.Invoke();
            viewInstance.Hide();
            viewInstance = null;
        }

        public bool OnBack()
        {
            if (DefaultAction != null)
            {
                OnAction(DefaultAction);
            }
            return true;
        }
    }
}