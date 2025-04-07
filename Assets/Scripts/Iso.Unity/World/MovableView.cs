using Common.Unity.Bind;
using Common.Unity.Util;
using Common.Util.Math;
using Iso.Movables;
using Spine;
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
        
        [SerializeField, Range(0f, 10f)]
        private float _velocity;
        
        public float Velocity
        {
            get => _velocity;
            set
            {
                _velocity = value;
                if (Model != null)
                {
                    Model.velocity = _velocity;
                }
            }
        }
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            Velocity = _velocity; // Вызывает сеттер и обновляет Model.velocity
        }
#endif
        
        /// <summary>
        /// back (rear) animation of movable, should be heading East (right-top iso)   
        /// </summary>
        public SkeletonAnimation back;

        private SkeletonAnimation CurrentAnimation => front.isActiveAndEnabled ? front : back;

        public override void OnBind()
        {
            base.OnBind();
            if (front == null && back == null)
            {
                var path = "Visitors/" + Model.Info.Id;
                var prefab = Resources.Load<MovableView>(path);
                var obj = Instantiate(prefab, transform);
                front = obj.front;
                back = obj.back;
            }
            BindModelEvents(Model.Events, evt =>
            {
                switch (evt)
                {
                    case MovableEvent.dirChange:
                        OnDirChange();
                        break;
                    case MovableEvent.movingChange:
                        OnMovingChange();
                        break;
                    case MovableEvent.selectedChange:
                        OnSelectedChange();
                        break;
                }
            });
            OnDirChange();
            OnMovingChange();
        }

        private void OnSelectedChange()
        {
            
        }

        private void OnMovingChange()
        {
            front.AnimationName = back.AnimationName = Model.Moving ? ANIM_WALK : ANIM_IDLE;
        }

        private void OnDirChange()
        {
            var dir = Model.Dir;
            var fwd = dir is Dir.S or Dir.W;
            var flip = dir is Dir.W or Dir.N;
            front.SetActive(fwd);
            back.SetActive(!fwd);
            transform.localScale = new Vector3(flip ? -1 : 1, 1, 1);
        }

        private void Update()
        {
            if (IsBound())
            {
                this.ApplyTransform(Model.pos);
            }
        }

        private static readonly SkeletonBounds Bounds = new();

        public bool HitTest(Vector3 worldPoint)
        {
            Bounds.Update(CurrentAnimation.skeleton, true);
            return Bounds.ContainsPoint(worldPoint.x, worldPoint.y) != null;
        }

    }
}