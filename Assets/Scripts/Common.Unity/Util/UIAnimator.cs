using System;
using Common.Lang.Entity;
using Common.Unity.Boot;
using Common.Util.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.Util
{
    /// <summary>
    /// ui animator
    /// </summary>
    public class UIAnimator : GenericBean
    {
        /// <summary>
        /// action to invoke on all animations complete
        /// </summary>
        private readonly Action onComplete;
        
        /// <summary>
        /// show/hide animation flag
        /// </summary>
        private readonly bool show;

        /// <summary>
        /// animation duration
        /// </summary>
        public float Duration = DefaultAnimator.AnimationTime;

        public LeanTweenType EaseShow = LeanTweenType.easeOutCubic;
        
        public LeanTweenType EaseHide = LeanTweenType.easeInCubic;
        
        /// <summary>
        /// count of running animations
        /// </summary>
        private int animationCount;

        /// <summary>
        /// we have to disable layout on root component to prevent tween/layout race
        /// </summary>
        private LayoutGroup layoutGroup;

        public UIAnimator(Component root, bool show, Action onComplete = null)
        {
            this.show = show;
            this.onComplete = onComplete;
            LayoutRebuilder.ForceRebuildLayoutImmediate(root.transform as RectTransform);
            layoutGroup = root.gameObject.GetComponent<LayoutGroup>();
            if (layoutGroup != null)
            {
                layoutGroup.enabled = false;
            }
        }
        
        /// <summary>
        /// start component move (slide) animation which will move component to/from offscreen
        /// </summary>
        /// <param name="component">a component to animate</param>
        /// <param name="dirOut">direction of component offscreen position</param>
        /// <returns></returns>
        public UIAnimator Move(Component component, Dir dirOut)
        {
            animationCount++;
            var tx = component.transform;
            var rt = tx as RectTransform;
            var screenRect = rt.ToScreenSpace();
            var posShow = tx.position;
            var delta = screenRect.size * new Vector2(dirOut.X(), dirOut.Y());
            var posHide = posShow + new Vector3(delta.x, delta.y, 0);
            var posTo = posHide;
            var ease = EaseHide;
            var lc = component.gameObject.AddComponent<LayoutCtrl>();
            
            if (show)
            {
                tx.position = posHide;
                posTo = posShow;
                ease = EaseShow;
            }
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"{Time.frameCount}:{component.name}:{show}:started: delta={delta}, {tx.position} > {posTo}");
            }

            LeanTween.move(component.gameObject, posTo, Duration)
                .setEase(ease)
                //.setOnUpdate(OnUpdate)
                .setOnComplete(() =>
                {
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug($"{component.name}:{show}:finished");
                    }
                    GameObject.Destroy(lc);
                    OnTweenComplete();
                })
                .updateNow();
            return this;
        }

        private void OnTweenComplete()
        {
            if (--animationCount == 0 && onComplete != null)
            {
                //
                // have to run on next frame to let tween cleanup
                Unicom.RunNextTime(() =>
                {
                    if (layoutGroup != null)
                    {
                        layoutGroup.enabled = true;
                    }

                    onComplete();
                });
            }
        }
    }
}
