using System.Collections.Generic;
using Common.Util.Math;
using Math;

namespace Common.Unity.Util.Math
{
    public class IsometricBoundsComparator : IComparer<RectFloat>
    {
        public int Compare(RectFloat b1, RectFloat b2)
        {
            var q = b1.getQuadrant(b2!.x, b2.y, Dir.NE);
            var result = GetResult(q);
            if (result == 0) {
                q = b1.getQuadrant(b2.GetMaxX(), b2.GetMaxY(), Dir.SW);
                result = GetResult(q);
            }
            return result;

        }

        private static int GetResult(Dir dir)
        {
            switch (dir)
            {
                case Dir.E:
                case Dir.N:
                case Dir.NE:
                    return -1;
                case Dir.S:
                case Dir.SW:
                case Dir.W:
                    return 1;
                default:
                    return 0;
            }
        }
    }
}