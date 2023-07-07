using Common.Util.Math;
using UnityEngine;

namespace Common.Unity.Util.Math
{
    public static class UnityMathEx
    {
        public static Vector2 v2m(this IsometricProjector prj, Vector3 viewPos)
        {
            return new Vector2(prj.v2mx(viewPos.x, viewPos.y), prj.v2my(viewPos.x, viewPos.y));
        }
        
        public static Vector2 m2v(this IsometricProjector prj, Vector3 modelPos)
        {
            return new Vector2(prj.m2vx(modelPos.x, modelPos.y), prj.m2vy(modelPos.x, modelPos.y));
        }
        
        public static Vector2 m2v(this IsometricProjector prj, Vector2Int modelPos)
        {
            return new Vector2(prj.m2vx(modelPos.x, modelPos.y), prj.m2vy(modelPos.x, modelPos.y));
        }
    }
}