using System;
using System.Text;
using Common.Api.System;
using Common.Unity.Bind;
using Common.Unity.Boot;
using Common.Util;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.UI.Debug
{
    /// <summary>
    /// ui for general system info
    /// </summary>
    public class SystemDebugView : BindableMono<UnicomDebug>
    {
        public TMP_InputField Info;
        
        public Button httpButton;
        
        public Button crashButton;
        
        public Button systemInfoButton;
        
        public Button reloadPlayerButton;

        public override void OnBind()
        {
            Info.text =
                "App ID: " + Application.identifier + "\n" +
                "LocalIPv4: " + SystemApi.GetLocalIP() + "\n" +
                "";
            BindClick(httpButton, () =>
                Application.OpenURL("http://localhost:" + Model.HttpDebug.Server.Port));
            BindClick(crashButton, () => throw new Exception("crash at " + DateTime.Now));
            BindClick(systemInfoButton, ShowSystemInfo);
            BindClick(reloadPlayerButton, Unicom.Instance.Reload);
        }

        private void ShowSystemInfo()
        {
            var sb = new StringBuilder();
            sb.AppendKeyValueLine("genuine", Application.genuine);
            new DialogMessage
            {
                Title = "System info",
                Message = sb.ToString()
            }.AddActionOk().Show();
        }
    }
}