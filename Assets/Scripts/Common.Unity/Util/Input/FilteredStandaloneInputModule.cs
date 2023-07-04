using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Unity.Util.Input
{
    /// <summary>
    /// StandaloneInputModule extension with input lock support,
    /// most of code here is copy/paste from StandaloneInputModule due to extension problems
    /// </summary>
    public class FilteredStandaloneInputModule : StandaloneInputModule
    {
        public event EventHandler<RaycastResultEventArgs> RaycastResultFilter;
        
        public override void Process()
        {
            if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
                return;

            bool usedEvent = SendUpdateEventToSelectedObject();

            // case 1004066 - touch / mouse events should be processed before navigation events in case
            // they change the current selected gameobject and the submit button is a touch / mouse button.

            // touch needs to take precedence because of the mouse emulation layer
            if (!ProcessTouchEvents() && input.mousePresent)
                ProcessMouseEvent();

            if (eventSystem.sendNavigationEvents)
            {
                if (!usedEvent)
                    usedEvent |= SendMoveEventToSelectedObject();

                if (!usedEvent)
                    SendSubmitEventToSelectedObject();
            }
        }
        
        private bool ShouldIgnoreEventsOnNoFocus()
        {
            switch (SystemInfo.operatingSystemFamily)
            {
                case OperatingSystemFamily.Windows:
                case OperatingSystemFamily.Linux:
                case OperatingSystemFamily.MacOSX:
#if UNITY_EDITOR
                    if (UnityEditor.EditorApplication.isRemoteConnected)
                        return false;
#endif
                    return true;
                default:
                    return false;
            }
        }
        
        private bool ProcessTouchEvents()
        {
            for (int i = 0; i < input.touchCount; ++i)
            {
                Touch touch = input.GetTouch(i);

                if (touch.type == TouchType.Indirect)
                    continue;

                bool released;
                bool pressed;
                var pointer = GetTouchPointerEventData(touch, out pressed, out released);

                ProcessTouchPress(pointer, pressed, released);

                if (!released)
                {
                    ProcessMove(pointer);
                    ProcessDrag(pointer);
                }
                else
                    RemovePointerData(pointer);
            }
            return input.touchCount > 0;
        }
        
        protected new PointerEventData GetTouchPointerEventData(Touch input, out bool pressed, out bool released)
        {
            PointerEventData pointerData;
            var created = GetPointerData(input.fingerId, out pointerData, true);

            pointerData.Reset();

            pressed = created || (input.phase == TouchPhase.Began);
            released = (input.phase == TouchPhase.Canceled) || (input.phase == TouchPhase.Ended);

            if (created)
                pointerData.position = input.position;

            if (pressed)
                pointerData.delta = Vector2.zero;
            else
                pointerData.delta = input.position - pointerData.position;

            pointerData.position = input.position;

            pointerData.button = PointerEventData.InputButton.Left;

            if (input.phase == TouchPhase.Canceled)
            {
                pointerData.pointerCurrentRaycast = new RaycastResult();
            }
            else
            {
                eventSystem.RaycastAll(pointerData, m_RaycastResultCache);
                
                RaycastResultFilter?.Invoke(this, new RaycastResultEventArgs(m_RaycastResultCache));
                
                var raycast = FindFirstRaycast(m_RaycastResultCache);
                pointerData.pointerCurrentRaycast = raycast;
                m_RaycastResultCache.Clear();
            }
            return pointerData;
        }
        
        private readonly MouseState m_MouseState = new MouseState();
        
        protected override MouseState GetMousePointerEventData(int id)
        {
            // UnityEngine.Debug.Log("<color=cyan>GetMousePointerEventData</color>");
            // Populate the left button...
            PointerEventData leftData;
            var created = GetPointerData(kMouseLeftId, out leftData, true);
        
            leftData.Reset();
        
            if (created)
                leftData.position = input.mousePosition;
        
            Vector2 pos = input.mousePosition;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                // We don't want to do ANY cursor-based interaction when the mouse is locked
                leftData.position = new Vector2(-1.0f, -1.0f);
                leftData.delta = Vector2.zero;
            }
            else
            {
                leftData.delta = pos - leftData.position;
                leftData.position = pos;
            }
            leftData.scrollDelta = input.mouseScrollDelta;
            leftData.button = PointerEventData.InputButton.Left;
            eventSystem.RaycastAll(leftData, m_RaycastResultCache);
            
            RaycastResultFilter?.Invoke(this, new RaycastResultEventArgs(m_RaycastResultCache));
            
            var raycast = FindFirstRaycast(m_RaycastResultCache);
            leftData.pointerCurrentRaycast = raycast;
            m_RaycastResultCache.Clear();
        
            // copy the apropriate data into right and middle slots
            PointerEventData rightData;
            GetPointerData(kMouseRightId, out rightData, true);
            CopyFromTo(leftData, rightData);
            rightData.button = PointerEventData.InputButton.Right;
        
            PointerEventData middleData;
            GetPointerData(kMouseMiddleId, out middleData, true);
            CopyFromTo(leftData, middleData);
            middleData.button = PointerEventData.InputButton.Middle;
        
            m_MouseState.SetButtonState(PointerEventData.InputButton.Left, StateForMouseButton(0), leftData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Right, StateForMouseButton(1), rightData);
            m_MouseState.SetButtonState(PointerEventData.InputButton.Middle, StateForMouseButton(2), middleData);
        
            return m_MouseState;        
        }
    }
}