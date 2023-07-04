using System;
using Common.Api;
using UnityEngine.UI;

namespace Common.Unity.Api.Mono
{
    /// <summary>
    /// monoBehaviour actions hub
    /// </summary>
    public class MonoApi : AbstractApi
    {
        /// <summary>
        /// clients should register here for all the button click events
        /// </summary>
        public event Action<Button> OnButtonClick;
        
        /// <summary>
        /// this should be called on every button click before invoking click handler
        /// </summary>
        public void ButtonClicked(Button btn)
        {
            OnButtonClick?.Invoke(btn);
        }
    }
}
