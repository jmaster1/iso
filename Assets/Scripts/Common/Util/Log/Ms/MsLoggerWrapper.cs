using System;
using Microsoft.Extensions.Logging;

namespace Common.Util.Log
{
    public class MsLoggerWrapper : LogWrapper
    {
        private readonly ILogger _logger;

        public MsLoggerWrapper(ILogger logger)
        {
            _logger = logger;
        }

        protected override void DebugImpl(string message) => _logger.LogDebug(message);
        protected override void DebugImpl(Exception exception, string message) => _logger.LogDebug(exception, message);
        protected override void DebugFormatImpl(string format, params object[] args) => _logger.LogDebug(format, args);

        protected override void InfoImpl(string message) => _logger.LogInformation(message);
        protected override void InfoImpl(Exception exception, string message) => _logger.LogInformation(exception, message);
        protected override void InfoFormatImpl(string format, params object[] args) => _logger.LogInformation(format, args);

        protected override void WarnImpl(string message) => _logger.LogWarning(message);
        protected override void WarnImpl(Exception exception, string message) => _logger.LogWarning(exception, message);
        protected override void WarnFormatImpl(string format, params object[] args) => _logger.LogWarning(format, args);

        protected override void ErrorImpl(string message) => _logger.LogError(message);
        protected override void ErrorImpl(Exception exception, string message) => _logger.LogError(exception, message);
        protected override void ErrorFormatImpl(string format, params object[] args) => _logger.LogError(format, args);

        protected override void FatalImpl(string message) => _logger.LogCritical(message);
        protected override void FatalImpl(Exception exception, string message) => _logger.LogCritical(exception, message);
        protected override void FatalFormatImpl(string format, params object[] args) => _logger.LogCritical(format, args);

        public override bool IsDebugEnabled => _logger.IsEnabled(LogLevel.Debug);
        public override bool IsInfoEnabled => _logger.IsEnabled(LogLevel.Information);
        public override bool IsWarnEnabled => _logger.IsEnabled(LogLevel.Warning);
        public override bool IsErrorEnabled => _logger.IsEnabled(LogLevel.Error);
        public override bool IsFatalEnabled => _logger.IsEnabled(LogLevel.Critical);
    }
}