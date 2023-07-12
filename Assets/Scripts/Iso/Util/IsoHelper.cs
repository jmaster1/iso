namespace Iso.Util
{
    public static class IsoHelper
    {
        private const double TOLERANCE = 0.0001f;
        
        public static bool FloatEquals(this float a, float b)
        {
            return System.Math.Abs(a - b) < TOLERANCE;
        }
    }
}