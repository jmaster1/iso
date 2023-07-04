using Common.Unity.Util;
using UnityEditor;
using UnityEngine;

namespace Common.Editor.Utils
{
    /// <summary>
    /// Detects scene hierarchy change, finds all new canvases,
    /// and sets currently active camera, if camera not already set
    /// </summary>
    [InitializeOnLoad]
    public class CanvasDetector
    {

        static CanvasDetector()
        {
            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        }

        private static void OnHierarchyChanged()
        {
            GameObject activeObj = Selection.activeGameObject;
            if (activeObj == null) return;
            Camera camera = Camera.main;
            Canvas[] canvases = activeObj.GetComponentsInChildren<Canvas>();
            foreach (Canvas canvas in canvases)
            {
                if (!canvas.worldCamera && !canvas.gameObject.IsPrefab())
                {
                    canvas.worldCamera = camera;
                }
            }
        }
    }
    
}