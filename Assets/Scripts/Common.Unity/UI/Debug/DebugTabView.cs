using Common.Unity.Bind;
using Common.Unity.Boot;
using Common.Unity.Util;
using UnityEngine.UI;

namespace Common.Unity.UI.Debug
{
    /// <summary>
    /// represents tab button in debug view
    /// </summary>
    public class DebugTabView : BindableMono<DebugTab>
    {
        /// <summary>
        /// tab button
        /// </summary>
        public Button button;
        
        /// <summary>
        /// instantiated tab content view 
        /// </summary>
        internal BindableMonoRaw view;

        public override void OnBind()
        {
            button.SetText(Model.Label);
            BindClick(button, Model.Select);
            BindInteractable(Model.selected, button, true);
        }
    }
}
