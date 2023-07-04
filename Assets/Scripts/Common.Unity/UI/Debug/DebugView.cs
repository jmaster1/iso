using Common.Unity.Bind;
using Common.Unity.Boot;
using Common.Unity.Util;
using Common.Unity.View;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.UI.Debug
{
    /// <summary>
    /// debug popup containing tabbed panel with all the debug views
    /// </summary>
    public class DebugView : BindableMono<UnicomDebug>
    {
        [DefaultButton]
        public Button closeButton;
        
        /// <summary>
        /// content for active tab view
        /// </summary>
        public Transform Content;

        /// <summary>
        /// tab buttons list adapter
        /// </summary>
        public DebugTabListAdapter tabs;
        
        /// <summary>
        /// currently displaying tab view
        /// </summary>
        private BindableMonoRaw currentView;

        public override void OnBind()
        {
            BindClick(closeButton, () => Model.DebugView.Hide());
            BindBindable(Model.tabs, tabs);
            BindToHolder(Model.tabSelection.Selected, tab =>
            {
                currentView?.SetActive(false);
                var tabView = tabs.FindView(tab);
                //
                // create view if not yet
                if (tabView.view == null)
                {
                    tabView.view = ViewPoolApi.AddView(tab.ViewType, tab.Model, Content);
                }

                currentView = tabView.view;
                currentView.SetActive(true);
            });
        }

        public override bool OnBack()
        {
            Model.DebugView.Hide();
            return true;
        }
    }
}
