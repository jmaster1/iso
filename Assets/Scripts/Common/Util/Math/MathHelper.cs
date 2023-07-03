using UnityEngine;

namespace Common.Util.Math
{
    public static class MathHelper
    {
        /// <summary>
        /// remap value from range [fromMin, fromMax] to [toMin, toMax]
        /// </summary>
        public static float Remap(this float val, float fromMin, float fromMax, float toMin, float toMax)
        {
            var t = Mathf.InverseLerp(fromMin, fromMax, val);
            return Mathf.Lerp(toMin, toMax, t);
        }
        
        /// <summary>
        /// check if float is int
        /// </summary>
        public static bool IsInt(this float val)
        {
            return val % 1 == 0;
        }
        
        /// <summary>
        /// check if both components are int
        /// </summary>
        public static bool IsInt(this Vector2 val)
        {
            return val.x.IsInt() && val.y.IsInt();
        }
        
        /// <summary>
        /// check if any component is int
        /// </summary>
        public static bool IsIntAny(this Vector2 val)
        {
            return val.x.IsInt() || val.y.IsInt();
        }
        
        public static int Round(this float val)
        {
            return (int)System.Math.Round(val);
        }
        
        public static int ToInt(this bool val)
        {
            return val ? 1 : 0;
        }
        
        public static float Abs(this float val)
        {
            return System.Math.Abs(val);
        }
        
        public static int Floor(this float val)
        {
            return (int)System.Math.Floor(val);
        }
        
        public static int Ceil(this float val)
        {
            return (int)System.Math.Ceiling(val);
        }
        
        public static Vector2Int Round(this Vector2 val)
        {
            return new Vector2Int(val.x.Round(), val.y.Round());
        }
        
        public static float DistanceTo(this Vector2 val, Vector2 to)
        {
            return (val - to).Len();
        }
        
        public static Dir Dir(this Vector2 val)
        {
            return DirEx.Resolve(val);
        }
        
        public static bool IsZero(this Vector2 val)
        {
            return val.x == 0 && val.y == 0;
        }
        
        public static bool IsZeroAny(this Vector2 val)
        {
            return val.x == 0 || val.y == 0;
        }
        
        public static Vector2 Mod(this Vector2 val, float mod)
        {
            val.Set(val.x % mod, val.y % mod);
            return val;
        }
        
        public static float Len(this Vector2 val)
        {
            if (val.x == 0)
            {
                return System.Math.Abs(val.y);
            }
            if (val.y == 0)
            {
                return System.Math.Abs(val.x);
            }
            return val.magnitude;
        }
        
        public static Vector2 SetLen(this Vector2 val, float len)
        {
            if (len == 0 || val.IsZero())
            {
                return Vector2.zero;
            }
            float scl = len / val.Len();
            return val * scl;
        }
        
        /// <summary>
        /// retrieve minimum int rectangle that contains given rect 
        /// </summary>
        public static RectInt RoundExpand(this Rect rc)
        {
            int x0 = rc.xMin.Floor();
            int y0 = rc.yMin.Floor();
            int x1 = rc.xMax.Ceil();
            int y1 = rc.yMax.Ceil();
            int w = x1 - x0;
            int h = y1 - y0;
            return new RectInt(x0, y0, w, h);
        }
        
        /// <summary>
        /// retrieve rounded int rectangle 
        /// </summary>
        public static RectInt Round(this Rect rc)
        {
            return new RectInt(rc.x.Round(), rc.y.Round(), rc.width.Round(), rc.height.Round());
        }

        /// <summary>
        /// retrieve rect size in given direction
        /// </summary>
        /// <param name="rc"></param>
        /// <param name="dir">must be primary</param>
        /// <returns>width or height</returns>
        public static int GetSize(this RectInt rc, Dir dir)
        {
            LangHelper.Validate(dir.IsPrimary());
            return dir.IsHorz() ? rc.width : rc.height;
        }

        public static RectInt Expand(this RectInt rc, int delta)
        {
            return new RectInt(rc.xMin - delta, rc.yMin - delta,
                rc.xMax + delta * 2, rc.yMax + delta);
        }
        
        public static Rect ToRect(this RectInt rc)
        {
            return new Rect(rc.xMin, rc.yMin, rc.width, rc.height);
        }
        
        public static Rect Expand(this Rect rc, float delta)
        {
            return new Rect(rc.xMin - delta, rc.yMin - delta,
                rc.xMax + delta * 2, rc.yMax + delta);
        }
        
        public static RectInt Intersect(this RectInt a, RectInt b)
        {
            int x0 = Mathf.Max(a.xMin, b.xMin);
            int y0 = Mathf.Max(a.yMin, b.yMin);
            int x1 = Mathf.Min(a.xMax, b.xMax);
            int y1 = Mathf.Min(a.yMax, b.yMax);
            int w = x1 - x0;
            int h = y1 - y0;
            return new RectInt(x0, y0, w, h);
        }
        
        public static Rect Intersect(this Rect a, Rect b)
        {
            var x0 = Mathf.Max(a.xMin, b.xMin);
            var y0 = Mathf.Max(a.yMin, b.yMin);
            var x1 = Mathf.Min(a.xMax, b.xMax);
            var y1 = Mathf.Min(a.yMax, b.yMax);
            var w = x1 - x0;
            var h = y1 - y0;
            return new Rect(x0, y0, w, h);
        }

        public static Rect Intersect(this Rect a, RectInt b)
        {
            return a.Intersect(b.ToRect());
        }

        public static bool Contains(this Rect a, Rect b)
        {
            return b.xMin >= a.xMin && b.xMax <= a.xMax &&
                   b.yMin >= a.yMin && b.yMax <= a.yMax;
        }
        
        public static bool Contains(this Rect a, RectInt b)
        {
            return b.xMin >= a.xMin && b.xMax <= a.xMax &&
                   b.yMin >= a.yMin && b.yMax <= a.yMax;
        }
        
        public static bool Contains(this RectInt a, Rect b)
        {
            return b.xMin >= a.xMin && b.xMax <= a.xMax &&
                   b.yMin >= a.yMin && b.yMax <= a.yMax;
        }
        
        public static bool Contains(this RectInt a, RectInt b)
        {
            return b.xMin >= a.xMin && b.xMax <= a.xMax &&
                   b.yMin >= a.yMin && b.yMax <= a.yMax;
        }

        public static RectInt MoveBy(this RectInt rc, Vector2 v)
        {
            return new RectInt((int) (rc.x + v.x), 
                (int) (rc.y + v.y), rc.width, rc.height);
        }
        
        public static RectInt MoveTo(this RectInt rc, Vector2 v)
        {
            return new RectInt((int)v.x, (int)v.y, rc.width, rc.height);
        }
        
        public static float Lerp(float min, float max, float scale) {
            return min + (max - min) * scale;
        }
    }
}