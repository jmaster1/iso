using Common.Unity.Bind;
using Common.Util.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.Util
{
    public class LayoutCtrl : BindableMono<object>, ILayoutSelfController
    {
        public void SetLayoutHorizontal()
        {
            Log.Debug($"{Time.frameCount}:::::::::::::::::::::{name}:{transform.position.y.Round()}");
        }

        public void SetLayoutVertical()
        {
            //Log.Debug(name);
        }
    }
}