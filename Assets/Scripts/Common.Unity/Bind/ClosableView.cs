using Common.Player;
using Common.Unity.View;
using UnityEngine.UI;

namespace Common.Unity.Bind
{
    /// <summary>
    /// base class for views with close button
    /// </summary>
    /// <typeparam name="T">view controller type</typeparam>
    public class ClosableView<T> : BindableMono<T> where T : IViewController
    {
        [DefaultButton]
        public Button closeButton;
        
        public override void OnBind()
        {
            if (closeButton != null)
            {
                BindClick(closeButton, Model.OnClose);
            }
        }
    }
}
