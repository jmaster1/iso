using System;
using log4net;

namespace Common.Util.Log
{
    public class Log4NetWrapper : LogWrapper
    {
        private readonly ILog _log;

        public Log4NetWrapper(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        protected override void DebugImpl(string message) => _log.Debug(message);
        protected override void DebugImpl(Exception exception, string message) => _log.Debug(message, exception);
        protected override void DebugFormatImpl(string format, params object[] args) => _log.DebugFormat(format, args);

        protected override void InfoImpl(string message) => _log.Info(message);
        protected override void InfoImpl(Exception exception, string message) => _log.Info(message, exception);
        protected override void InfoFormatImpl(string format, params object[] args) => _log.InfoFormat(format, args);

        protected override void WarnImpl(string message) => _log.Warn(message);
        protected override void WarnImpl(Exception exception, string message) => _log.Warn(message, exception);
        protected override void WarnFormatImpl(string format, params object[] args) => _log.WarnFormat(format, args);

        protected override void ErrorImpl(string message) => _log.Error(message);
        protected override void ErrorImpl(Exception exception, string message) => _log.Error(message, exception);
        protected override void ErrorFormatImpl(string format, params object[] args) => _log.ErrorFormat(format, args);

        protected override void FatalImpl(string message) => _log.Fatal(message);
        protected override void FatalImpl(Exception exception, string message) => _log.Fatal(message, exception);
        protected override void FatalFormatImpl(string format, params object[] args) => _log.FatalFormat(format, args);

        public override bool IsDebugEnabled => _log.IsDebugEnabled;
        public override bool IsInfoEnabled => _log.IsInfoEnabled;
        public override bool IsWarnEnabled => _log.IsWarnEnabled;
        public override bool IsErrorEnabled => _log.IsErrorEnabled;
        public override bool IsFatalEnabled => _log.IsFatalEnabled;
    }
}
