using UnityEditor;
using UnityEngine;

public class IsoMapEditor {
    /*
//You can see this under GameObject/UI
//Grouped together with the UI components
    [MenuItem("GameObject/UI/Text Area", false, 10)]
    public static void CreateTextArea(){
        GameObject go = new GameObject("Name");
    }
//You can see this under GameObject
    [MenuItem("GameObject/My Custom Contex Menu Item 1/Subitem 1", false, 10)]
    public static void CreateTextArea2(){
        GameObject go = new GameObject("Name");
    }
 
    [MenuItem("GameObject/My Custom Contex Menu Item 1/Subitem 2", false, 10)]
    public static void CreateTextArea3(){
        GameObject go = new GameObject("Name");
    }
 
    [MenuItem("GameObject/My Custom Contex Menu Item 2", false, 10)]
    public static void CreateTextArea5(){
        GameObject go = new GameObject("Name");
        GameObject obj = Selection.activeGameObject;
        obj.transform.Rotate(Vector3.up * 45);
    }
 */
    
    [MenuItem("GameObject/-----------------To front", false, 10)]
    public static void ToFront() {
        var obj = Selection.activeGameObject;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingOrder += 1;
        }
        var sa = obj.GetComponent<MeshRenderer>();
        if (sa != null)
        {
            sa.sortingOrder += 1;
        }
        Debug.Log(obj.name);
    }
}

