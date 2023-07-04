using Common.Lang.Observable;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Unity.Util.Input
{
    /// <summary>
    /// should be used along with selectable component when required to detect its' pressed state
    /// </summary>
    public class UIPresser : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public readonly BoolHolder Pressed = new BoolHolder();

        public void OnPointerDown(PointerEventData eventData)
        {
            Pressed.SetTrue();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Pressed.SetFalse();
        }
    }
}