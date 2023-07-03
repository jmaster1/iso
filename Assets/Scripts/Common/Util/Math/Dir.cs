using UnityEngine;

namespace Common.Util.Math
{
    public enum Dir
    {
        C,
        N,
        E,
        S,
        W,
        NE,
        SE,
        SW,
        NW
    }

    public static class DirEx
    {
        public static readonly Dir[] Primary = {Dir.N, Dir.E, Dir.S, Dir.W};
        public static readonly Dir[] Secondary = {Dir.NE, Dir.SE, Dir.SW, Dir.NW};
        public static readonly Dir[] Around = {Dir.N, Dir.NE, Dir.E, Dir.SE, 
            Dir.S, Dir.SW, Dir.W, Dir.NW};
    
        public static int X(this Dir e)
        {
            switch (e)
            {
                case Dir.C:
                    return 0;
                case Dir.N:
                    return 0;
                case Dir.E:
                    return 1;
                case Dir.S:
                    return 0;
                case Dir.W:
                    return -1;
                case Dir.NE:
                    return 1;
                case Dir.SE:
                    return 1;
                case Dir.SW:
                    return -1;
                case Dir.NW:
                    return -1;
            }
            return default;
        }
    
        public static int Y(this Dir e)
        {
            switch (e)
            {
                case Dir.C:
                    return 0;
                case Dir.N:
                    return 1;
                case Dir.E:
                    return 0;
                case Dir.S:
                    return -1;
                case Dir.W:
                    return 0;
                case Dir.NE:
                    return 1;
                case Dir.SE:
                    return -1;
                case Dir.SW:
                    return -1;
                case Dir.NW:
                    return 1;
            }
            return default;
        }

        public static Vector2Int Vector2Int(this Dir e)
        {
            return new Vector2Int(e.X(), e.Y());
        }
    
        public static Vector2 Vector2(this Dir e)
        {
            return new Vector2(e.X(), e.Y());
        }

        /// <summary>
        /// resolve direction from vector
        /// </summary>
        public static Dir Resolve(Vector2 v2)
        {
            var dx = System.Math.Sign(v2.x);
            var dy = System.Math.Sign(v2.y);
            if(dx == 0 && dy == 0) {
                return Dir.C;
            }
            if(dx == 0) {
                return dy < 0 ? Dir.S : Dir.N;
            }
            if(dy == 0) {
                return dx < 0 ? Dir.W : Dir.E;
            }
            if(dx < 0)
            {
                if(dy < 0) {
                    return Dir.SW;
                }
                return Dir.NW;
            }
            if(dy < 0) {
                return Dir.SE;
            }
            return Dir.NE;
        }

        /// <summary>
        /// inverse (rotated by 180 degrees) direction retrieval
        /// </summary>
        public static Dir Invert(this Dir dir)
        {
            switch (dir)
            {
                case Dir.N:
                    return Dir.S;
                case Dir.W:
                    return Dir.E;
                case Dir.S:
                    return Dir.N;
                case Dir.E:
                    return Dir.W;
                case Dir.C:
                    return Dir.C;
                case Dir.NE:
                    return Dir.SW;
                case Dir.NW:
                    return Dir.SE;
                case Dir.SE:
                    return Dir.NW;
                case Dir.SW:
                    return Dir.NE;
            }
            return default;
        }

        public static bool IsPrimary(this Dir dir)
        {
            switch (dir)
            {
                case Dir.N:
                case Dir.W:
                case Dir.S:
                case Dir.E:
                    return true;
            }
            return false;
        }
    
        public static bool IsVert(this Dir dir)
        {
            return dir.Y() != 0;
        }
    
        public static bool IsHorz(this Dir dir)
        {
            return dir.X() != 0;
        }
    }
}