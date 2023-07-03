using System;

namespace Common.TimeNS
{
    public static class TimeHelper
    {
        public static string ToStringMmss(this TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"mm\:ss");
        }
        
        public static string ToStringHhmmss(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm\:ss");
        }
    }
}