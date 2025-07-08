using System;
using System.Diagnostics;
using Common.Util.Log.Ms;

namespace Common.Util.Log
{
    /// <summary>
    /// LogWrapper allows shrinking of debug and info logs from build
    /// by checking 'DEBUG" conditional (Scripting Define Symbol)
    /// </summary>
    public abstract class LogWrapper
    {

        public static Func<Type, LogWrapper> Factory;

        public static LogWrapper Create(Type type)
        {
            Factory ??= type1 =>
            {
                var unityDebugLogger = new UnityDebugLogger(type1.Name);
                return new MsLoggerWrapper(unityDebugLogger);
            };
            return Factory(type);
        }

        [Conditional("DEBUG")]
        public void Debug(string message) => DebugImpl(message);

        [Conditional("DEBUG")]
        public void Debug(Exception exception, string message) => DebugImpl(exception, message);

        [Conditional("DEBUG")]
        public void DebugFormat(string format, params object[] args) => DebugFormatImpl(format, args);

        [Conditional("DEBUG")]
        public void Info(string message) => InfoImpl(message);

        [Conditional("DEBUG")]
        public void Info(Exception exception, string message) => InfoImpl(exception, message);

        [Conditional("DEBUG")]
        public void InfoFormat(string format, params object[] args) => InfoFormatImpl(format, args);

        public void Warn(string message) => WarnImpl(message);

        public void Warn(Exception exception, string message) => WarnImpl(exception, message);

        public void WarnFormat(string format, params object[] args) => WarnFormatImpl(format, args);

        public void Error(string message) => ErrorImpl(message);

        public void Error(Exception exception, string message = null) => ErrorImpl(exception, message);

        public void ErrorFormat(string format, params object[] args) => ErrorFormatImpl(format, args);

        public void Fatal(string message) => FatalImpl(message);

        public void Fatal(Exception exception, string message) => FatalImpl(exception, message);

        public void FatalFormat(string format, params object[] args) => FatalFormatImpl(format, args);

        public abstract bool IsDebugEnabled { get; }
        public abstract bool IsInfoEnabled { get; }
        public abstract bool IsWarnEnabled { get; }
        public abstract bool IsErrorEnabled { get; }
        public abstract bool IsFatalEnabled { get; }

        // Impl hooks
        protected abstract void DebugImpl(string message);
        protected abstract void DebugImpl(Exception exception, string message);
        protected abstract void DebugFormatImpl(string format, params object[] args);

        protected abstract void InfoImpl(string message);
        protected abstract void InfoImpl(Exception exception, string message);
        protected abstract void InfoFormatImpl(string format, params object[] args);

        protected abstract void WarnImpl(string message);
        protected abstract void WarnImpl(Exception exception, string message);
        protected abstract void WarnFormatImpl(string format, params object[] args);

        protected abstract void ErrorImpl(string message);
        protected abstract void ErrorImpl(Exception exception, string message);
        protected abstract void ErrorFormatImpl(string format, params object[] args);

        protected abstract void FatalImpl(string message);
        protected abstract void FatalImpl(Exception exception, string message);
        protected abstract void FatalFormatImpl(string format, params object[] args);
    }
}