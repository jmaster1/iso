using Common.Unity.Bind;
using Common.Unity.Boot;
using Common.Unity.Util;
using Common.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.UI.Debug
{

    /// <summary>
    /// the button that shows DebugView,
    /// the text on button updated to fps each second
    /// </summary>
    public class DebugButtonView : BindableMono<UnicomDebug>
    {
        public Button button;

        public Image inputLockImage;
        
        public override void OnBind()
        {
            BindToHolder(InputApi.Lock, OnInputLockChange);
            BindClick(button, () => Model.DebugView.Show());
        }

        private void OnInputLockChange(bool locked)
        {
            var color = inputLockImage.color;
            color.a = locked ? 0.5f : 0;
            inputLockImage.color = color;
        }

        private const int UpdateFpsTimeout = 1;

        private float timeSum;

        private int frameSum;
        
        void Update ()
        {
            timeSum += Time.deltaTime;
            frameSum++;
            if (!(timeSum > UpdateFpsTimeout)) return;
            var fps = (int) (frameSum / timeSum);
            button.SetText(fps.ToStr());
            timeSum = 0;
            frameSum = 0;
        }
    }
}