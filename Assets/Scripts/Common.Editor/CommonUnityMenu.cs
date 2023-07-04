using Common.IO.FileSystem;
using UnityEditor;
using UnityEngine;

namespace Common.Editor {
    
    /// <summary>
    /// unity editor menu commands for unicom
    /// </summary>
    public static class CommonUnityMenu
    {
        [MenuItem("Tools/Unicom/Build all")]
        public static void BuildAll()
        {
            CommonUnityTasks.BuildAll();
        }
        
        [MenuItem("Tools/Unicom/Build localization")]
        public static void BuildLocalization()
        {
            CommonUnityTasks.BuildLocalization();
        }
        
        [MenuItem("Tools/Unicom/Build info")]
        public static void BuildInfo()
        {
            CommonUnityTasks.BuildInfo();
        }

        [MenuItem("Tools/Unicom/Show data in explorer")]
        public static void OpenPersistentDataPath()
        {
            EditorUtility.RevealInFinder(Application.persistentDataPath);
        }
        
        [MenuItem("Tools/Unicom/Clear data")]
        public static void ClearPersistentData()
        {
            var path = Application.persistentDataPath;
            var fs = new LocalFileSystem(path);
            fs.DeleteAll();
        }
    }
}