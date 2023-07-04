using System;
using Common.Unity.Util;
using Common.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.Bind
{
    /// <summary>
    /// BindableMonoRaw dialog adapter that should be added to modal dialog views
    /// </summary>
    public class DialogAdapter : MonoBehaviour
    {
        /// <summary>
        /// semi-opaque image for dimming content under dialog
        /// </summary>
        public Image BackgroundImage;
        
        /// <summary>
        /// button that blocks input and delegates click to parent.OnBack(),
        /// so dialog might be closed by clicking outside of window area
        /// </summary>
        public Button BackgroundButton;
        
        /// <summary>
        /// the frame containing dialog elements
        /// </summary>
        public GameObject Window;

        /// <summary>
        /// reference to parent BindableMonoRaw
        /// </summary>
        private BindableMonoRaw parentBindable;

        private void Awake()
        {
            parentBindable = gameObject.GetComponentInParent<BindableMonoRaw>();
            LangHelper.Validate(parentBindable != null);
            parentBindable.Animator = PlayAnimation;
            BackgroundButton.onClick.AddListener(OnBackgroundButtonClick);
        }

        private void OnBackgroundButtonClick()
        {
            parentBindable.OnBack();
        }

        public void PlayAnimation(GameObject target, bool show, Action onComplete)
        {
            if (BackgroundImage != null)
            {
                AnimateBackground(show);
            }
            DefaultAnimator.PlayAnimation(Window.gameObject, show, onComplete);
        }

        protected virtual void AnimateBackground(bool show)
        {
            var target = BackgroundImage.rectTransform;
            var animationTime = DefaultAnimator.AnimationTime;
            if (show)
            {
                var targetAlpha = BackgroundImage.SetAlpha(0);
                LeanTween.alpha(target, targetAlpha, animationTime);
            } else
            {
                var targetAlpha = BackgroundImage.color.a;
                LeanTween.alpha(target, 0, animationTime).setOnComplete(() =>
                {
                    BackgroundImage.SetAlpha(targetAlpha);
                });
            }
        }
    }
}
