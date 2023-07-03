using System;
using System.Diagnostics;
using log4net;

namespace Common.Util.Log
{
    /// <summary>
    /// LogWrapper allows shrinking of debug and info logs from build
    /// by checking 'DEBUG" conditional (Scripting Define Symbol)
    /// </summary>
    public class LogWrapper
    {
        private readonly ILog _log;

        public LogWrapper(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        [Conditional("DEBUG")]
        public void Debug(object message)
        {
            _log.Debug(message);
        }

        [Conditional("DEBUG")]
        public void Debug(object message, Exception exception)
        {
            _log.Debug(message, exception);
        }

        [Conditional("DEBUG")]
        public void DebugFormat(string format, params object[] args)
        {
            _log.DebugFormat(format, args);
        }

        [Conditional("DEBUG")]
        public void Info(object message)
        {
            _log.Info(message);
        }

        [Conditional("DEBUG")]
        public void Info(object message, Exception exception)
        {
            _log.Info(message, exception);
        }

        [Conditional("DEBUG")]
        public void InfoFormat(string format, params object[] args)
        {
            _log.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _log.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _log.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _log.FatalFormat(format, args);
        }

        public bool IsDebugEnabled => _log.IsDebugEnabled;

        public bool IsInfoEnabled => _log.IsInfoEnabled;

        public bool IsWarnEnabled => _log.IsWarnEnabled;

        public bool IsErrorEnabled => _log.IsErrorEnabled;

        public bool IsFatalEnabled => _log.IsFatalEnabled;
    }
}