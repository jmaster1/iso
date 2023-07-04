using Common.Lang;
using UnityEngine;

namespace Common.Unity.Util
{
    /// <summary>
    /// ITreeApi implementation for UnityEngine.Transform 
    /// </summary>
    public class TransformTreeApi : ITreeApi<Transform>
    {
        public static readonly TransformTreeApi Instance = new TransformTreeApi();
        
        public Transform GetParent(Transform e)
        {
            return e.parent;
        }

        public int GetSize(Transform e)
        {
            return e.childCount;
        }

        public Transform GetChild(Transform e, int index)
        {
            return e.GetChild(index);
        }
    }
}