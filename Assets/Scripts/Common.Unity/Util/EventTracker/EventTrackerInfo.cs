using Common.Lang;
using Common.Lang.Entity;

namespace Common.Unity.Util.EventTracker
{
    public class EventTrackerInfo : AbstractEntity
    {
        public string UrlProd;
        
        public string UrlDev;

        public string AuthKey;
        
        public string StoreFileName;

        public float FlushInterval;

        public float SendTimeout;
    }
}