using System;

namespace Common.Api.Notification
{
    public class Notification
    {
        public string Title;
        public string Text;
        public DateTime FireTime;
        public string SmallIcon;
        public TimeSpan? RepeatInterval;
        public string LargeIcon;
        //public Color? Color;
    }
}