using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Common.Unity.Util.Input
{
    public class RaycastResultEventArgs
    {
        public RaycastResultEventArgs(List<RaycastResult> s) { res = s; }
        
        public List<RaycastResult> res { get; } // readonly
    }
}