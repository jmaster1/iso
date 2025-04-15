namespace Common.Util.Math
{
    public static class MathHelper
    {
        /// <summary>
        /// check if float is int
        /// </summary>
        public static bool IsInt(this float val)
        {
            return val % 1 == 0;
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
        
        public static float Lerp(float min, float max, float scale) {
            return min + (max - min) * scale;
        }
    }
}