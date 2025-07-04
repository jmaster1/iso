using System;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Util.Log
{
    public class UnityDebugLogger : ILogger
    {
        private readonly string category;

        public UnityDebugLogger(string category)
        {
            this.category = category;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId,
            TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string message = formatter(state, exception);
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.Error:
                    Debug.LogError($"[{category}] {message}");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"[{category}] {message}");
                    break;
                default:
                    Debug.Log($"[{category}] {message}");
                    break;
            }
        }
    }
}