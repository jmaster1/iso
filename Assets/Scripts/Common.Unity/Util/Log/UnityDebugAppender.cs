using log4net.Appender;
using log4net.Core;

namespace Common.Unity.Util.Log
{
    /// <summary>
    /// log4net appender that writes to unity Debug.Log
    /// </summary>
    public class UnityDebugAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            var message = RenderLoggingEvent(loggingEvent);
            var level = loggingEvent.Level;
            if (level <= Level.Notice)
            {
                UnityEngine.Debug.Log(message);
                return;
            }
            if (level <= Level.Warn)
            {
                UnityEngine.Debug.LogWarning(message);
                return;
            }
            UnityEngine.Debug.LogError(message);
        }
    }
}