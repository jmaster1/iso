namespace Common.Util.Math
{
    // ReSharper disable InconsistentNaming
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
        public const int POSITIVE = 1;
        public const int NEGATIVE = -1;
        public const int ZERO = 0;
        public static readonly Dir[] Primary = {Dir.N, Dir.E, Dir.S, Dir.W};
        public static readonly Dir[] Secondary = {Dir.NE, Dir.SE, Dir.SW, Dir.NW};
        public static readonly Dir[] Around = {Dir.N, Dir.NE, Dir.E, Dir.SE, 
            Dir.S, Dir.SW, Dir.W, Dir.NW};
    
        public static int X(this Dir e)
        {
            return e switch
            {
                Dir.C => 0,
                Dir.N => 0,
                Dir.E => 1,
                Dir.S => 0,
                Dir.W => -1,
                Dir.NE => 1,
                Dir.SE => 1,
                Dir.SW => -1,
                Dir.NW => -1,
                _ => 0
            };
        }
        
        public static bool IsRight(this Dir e)
        {
            return e.X() == 1;
        }
        
        public static bool IsLeft(this Dir e)
        {
            return e.X() == -1;
        }
        
        public static bool IsUp(this Dir e)
        {
            return e.Y() == 1;
        }
        
        public static bool IsDown(this Dir e)
        {
            return e.Y() == -1;
        }
    
        public static int Y(this Dir e)
        {
            return e switch
            {
                Dir.C => 0,
                Dir.N => 1,
                Dir.E => 0,
                Dir.S => -1,
                Dir.W => 0,
                Dir.NE => 1,
                Dir.SE => -1,
                Dir.SW => -1,
                Dir.NW => 1,
                _ => 0
            };
        }

        /// <summary>
        /// resolve direction from vector
        /// </summary>
        public static Dir Resolve(float x, float y)
        {
            var dx = System.Math.Sign(x);
            var dy = System.Math.Sign(y);
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

        public static bool IsSameOrInverted(this Dir dir, Dir that)
        {
            return dir == that || dir.Invert() == that;
        }

        public static Dir ValueOf(int dx, int dy)
        {
            return dx switch
            {
                > 0 => dy switch
                {
                    > 0 => Dir.NE,
                    < 0 => Dir.SE,
                    _ => Dir.E
                },
                < 0 => dy switch
                {
                    > 0 => Dir.NW,
                    < 0 => Dir.SW,
                    _ => Dir.W
                },
                _ => dy switch
                {
                    > 0 => Dir.N,
                    < 0 => Dir.S,
                    _ => Dir.C
                }
            };
        }
    }
}