using System;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Iso.Movables;
using Spine.Unity;

namespace Iso.Unity.World
{
    public class MovableView : BindableMono<Movable>
    {
        public const string ANIM_WALK = "walk";
        public const string ANIM_IDLE = "idle";
        
        public SkeletonAnimation spine;
        private IsometricProjectorGrid prj;

        public override void OnBind()
        {
            base.OnBind();
            BindToHolder(Model.moving, moving => spine.AnimationName = moving ? ANIM_WALK : ANIM_IDLE);
            prj = UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
        }

        private void Update()
        {
            if (Model != null)
            {
                prj.Transform(gameObject, Model.pos);
            }
        }
    }
}