using Common.Util.Math;
using UnityEngine;
using UnityEngine.UI;

namespace Common.Unity.Util
{
    /// <summary>
    /// will update mask according to slider value
    /// </summary>
    public class SliderMaskController : MonoBehaviour
    {
        public Slider slider;
        
        public RectMask2D mask;

        private void Awake()
        {
            slider.onValueChanged.AddListener(OnSliderValueChange);
        }

        private void OnRectTransformDimensionsChange()
        {
            UpdateMask();
        }

        /// <summary>
        /// will update mask.pad.right to properly visualise front/back labels
        /// </summary>
        private void UpdateMask()
        {
            var width = mask.rectTransform.rect.width;
            var pad = mask.padding;
            pad.z = slider.value.Remap(slider.minValue, slider.maxValue, width, 0);
            mask.padding = pad;
        }

        private void OnSliderValueChange(float val)
        {
            UpdateMask();
        }
    }
}
