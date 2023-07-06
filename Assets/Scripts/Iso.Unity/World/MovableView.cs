using System;
using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Unity.Util.Math;
using Common.Util.Math;
using Iso.Movables;
using Spine.Unity;
using UnityEngine;

namespace Iso.Unity.World
{
    public class MovableView : BindableMono<Movable>
    {
        public const string ANIM_WALK = "walk";
        public const string ANIM_IDLE = "idle";
        
        /// <summary>
        /// front (face) animation of movable, should be heading South (right-bottom iso)   
        /// </summary>
        public SkeletonAnimation front;
        
        /// <summary>
        /// back (rear) animation of movable, should be heading East (right-top iso)   
        /// </summary>
        public SkeletonAnimation back;
        
        public IsometricProjectorGrid prj;


        public override void OnBind()
        {
            base.OnBind();
            BindModelEvents(Model.Events, evt =>
            {
                switch (evt)
                {
                    case MovableEvent.cellChange:
                        break;
                    case MovableEvent.pathEnd:
                        break;
                    case MovableEvent.teleportBegin:
                        break;
                    case MovableEvent.teleportEnd:
                        break;
                    case MovableEvent.dirChange:
                        OnDirChange(Model.Dir);
                        break;
                    case MovableEvent.movingChange:
                        OnMovingChange(Model.Moving);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(evt), evt, null);
                }
                Debug.Log("event=" + evt);
            });
            if (prj == null)
            {
                prj = UnityHelper.FindComponentInScene<IsometricProjectorGrid>();
            }
        }

        private void OnMovingChange(bool moving)
        {
            front.AnimationName = back.AnimationName = moving ? ANIM_WALK : ANIM_IDLE;
            Debug.Log("moving=" + moving);
        }

        private void OnDirChange(Dir dir)
        {
            var fwd = dir is Dir.S or Dir.W;
            var flip = dir is Dir.W or Dir.N;
            Debug.Log("Dir=" + dir);
            front.SetActive(fwd);
            back.SetActive(!fwd);
            transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
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