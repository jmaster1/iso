using UnityEditor;
using UnityEngine;

namespace Common.Editor
{

    public class EditorHelper
    {
        public static void ShowNotification(string text)
        {
            SceneView.lastActiveSceneView.ShowNotification(new GUIContent(text), .1f);
        }
    }

}