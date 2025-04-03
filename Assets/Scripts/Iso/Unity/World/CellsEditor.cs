using UnityEngine;

namespace Iso.Unity.World
{
    [ExecuteInEditMode]
    public class CellsEditor : MonoBehaviour
    {
        private void Update()
        {
            Debug.Log("Upd");
            
            if (Event.current != null && Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                Debug.Log("---------MUP");
            }
            
            if (Input.GetMouseButtonDown(0))
                Debug.Log("Pressed left-click.");
        }

        private void OnSceneGUI()
        {
            if (Event.current != null && Event.current.type == EventType.MouseUp && Event.current.button == 0) {
                Debug.Log("---------MUP");
            }
        }
    }
}
