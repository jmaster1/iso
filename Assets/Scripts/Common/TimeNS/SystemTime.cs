using System;

namespace Common.TimeNS
{
    /// <summary>
    /// Time extension that use local system time
    /// </summary>
    public class SystemTime : Time
    {
        /// <summary>
        /// current system time retrieval
        /// </summary>
        public static DateTime Current => DateTime.Now;
        
        public SystemTime()
        {
            Value = Current;
        }

        public override void Update()
        {
            Delta = Current - Value;
            Value = Current + Offset;
            Notify();
        }
    }
}