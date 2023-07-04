using System.Collections.Generic;
using Math;

namespace Common.Util.Math
{
    public class IsometricBoundsComparator : IComparer<RectFloat>
    {
        public int Compare(RectFloat b1, RectFloat b2)
        {
            if (b1 == null || b2 == null) return 0;
            var q = b1.GetQuadrant(b2.x, b2.y, Dir.NE);
            var result = GetResult(q);
            if (result != 0) return result;
            q = b1.GetQuadrant(b2.GetMaxX(), b2.GetMaxY(), Dir.SW);
            result = GetResult(q);
            return result;
        }

        private static int GetResult(Dir dir)
        {
            return dir switch
            {
                Dir.E => DirEx.NEGATIVE,
                Dir.N => DirEx.NEGATIVE,
                Dir.NE => DirEx.NEGATIVE,
                Dir.S => DirEx.POSITIVE,
                Dir.SW => DirEx.POSITIVE,
                Dir.W => DirEx.POSITIVE,
                _ => DirEx.ZERO
            };
        }
    }
}