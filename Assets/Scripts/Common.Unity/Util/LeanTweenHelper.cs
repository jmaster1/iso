using TMPro;
using UnityEngine.UI;

namespace Common.Unity.Util
{
    public static class LeanTweenHelper
    {

        public static LTDescr LeanAlpha(this Image image, float to, float time)
        {
            return LeanTween.alpha(image.rectTransform, to, time);
        }

        public static LTDescr LeanAlpha(this TMP_Text textMesh, float to, float time)
        {
            var color = textMesh.color;
            return LeanTween
                .value (textMesh.gameObject, color.a, to, time)
                .setOnUpdate (value => {
                    color.a = value;
                    textMesh.color = color;
                });
        }
        
    }
}