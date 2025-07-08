using System;
using System.Collections.Generic;
using System.Linq;
using Common.Util.Reflect;
using IsoNet.Core.Log.Appender;
using Microsoft.Extensions.Logging;

namespace IsoNet.Core.Log
{
    public abstract class AbstractLogger : ILogger
    {
        public IAppender? Appender;
    
        public string? Category;
    
        public abstract void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter);
    
        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        protected static T? ExtractParam<T, TState>(TState state, string key)
        {
            if (state is not IReadOnlyList<KeyValuePair<string, object>> kvps) return default;
            var match = kvps.FirstOrDefault(kv => kv.Key == key);
            if (match.Value is T typedValue)
                return typedValue;

            return default;
        }
    
        protected void Append(string text) => Appender?.Append(text);
    
        public static ILoggerProvider LoggerProvider<T>(IAppender appender) where T : AbstractLogger
        {
            return new GenericLoggerProvider<T>(appender);
        }
    }

    internal class GenericLoggerProvider<T> : ILoggerProvider where T : AbstractLogger
    {
        private readonly IAppender _appender;

        public GenericLoggerProvider(IAppender appender)
        {
            _appender = appender;
        }

        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            var logger = ReflectHelper.NewInstance<T>();
            logger.Appender = _appender;
            logger.Category = categoryName;
            return logger;
        }
    }
}
