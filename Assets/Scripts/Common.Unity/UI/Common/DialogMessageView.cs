using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Util;
using TMPro;
using UnityEngine.UI;

namespace Common.Unity.UI.Common
{
    /// <summary>
    /// unity ui/controller for DialogMessage
    /// </summary>
    public class DialogMessageView : BindableMono<DialogMessage>
    {
        public TextMeshProUGUI Title;
        
        public TMP_InputField Message;
        
        public Button ButtonExample;

        public override bool OnBack()
        {
            return Model.OnBack();
        }

        public override void OnBind()
        {
            Title.text = Model.Title;
            Message.text = Model.Message;
            // 
            // create buttons
            foreach (var text in Model.ActionNames)
            {
                var button = ButtonExample.Clone();
                button.SetActive(true);
                button.SetText(text);
                button.onClick.AddListener(delegate
                {
                    Model.OnAction(text);
                });
            }
        }

        public override void OnUnbind()
        {
            var buttons = gameObject.GetComponentsInChildren<Button>();
            foreach (var e in buttons)
            {
                if (e != ButtonExample)
                {
                    Destroy(e.gameObject);
                }
            }
            Title.text = Message.text = null;
        }
    }
}
