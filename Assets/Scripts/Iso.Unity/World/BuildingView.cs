using Common.Unity.Bind;
using Iso.Buildings;
using UnityEngine;

namespace Iso.Unity.World
{
    public class BuildingView : BindableMono<Building>
    {

        public override void OnBind()
        {
            base.OnBind();
            var path = "Buildings/" + Model.Info.Id;
            var prefab = Resources.Load<GameObject>(path);
            var obj = Instantiate(prefab, transform);
            if (Model.Flipped)
            {
                obj.transform.localScale = new Vector3(-1, 1);
            }
        }

        private void Update()
        {
            if (IsBound())
            {
                this.ApplyTransform(Model.X, Model.Y);
            }
        }
    }
}