using System;
using Common.Unity.Boot;
using UnityEngine;

namespace Common.Unity.Util
{
    /// <summary>
    /// responsible to play show/hide animation on game object
    /// </summary>
    public static class DefaultAnimator
    {
        /// <summary>
        /// default duration of show/hide animations
        /// </summary>
        public static float AnimationTime => Unicom.Instance.uiAnimationTime;

        public static Vector3 ScaleOut = new Vector3(1, 0, 1);
        
        public static void PlayAnimation(GameObject target, bool show, Action onComplete)
        {
            if (show)
            {
                target.transform.localScale = ScaleOut;
                LeanTween.scale(target, Vector3.one, AnimationTime)
                    .setEaseOutBack()
                    .setOnComplete(onComplete);
            } else {
                LeanTween.scale(target, ScaleOut, AnimationTime)
                    .setEaseInBack()
                    .setOnComplete(() =>
                    {
                        //
                        // restore scale
                        target.transform.localScale = Vector3.one;
                        onComplete?.Invoke();
                    });
            }
        }
    }
}