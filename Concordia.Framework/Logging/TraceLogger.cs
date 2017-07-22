using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concordia.Framework.Logging
{
    public class TraceLogger : ILogger
    {
        /// <summary>
        /// Gets the logger name.
        /// </summary>
        public string LoggerName { get; }

        /// <summary>
        /// Gets the minimum log level.
        /// </summary>
        public LogLevel MinimumLogLevel { get; }

        /// <summary>
        /// Initializes a new TraceLogger class.
        /// </summary>
        /// <param name="minimumLogLevel">The minimum log level.</param>
        /// <param name="loggerName">The logger name.</param>
        public TraceLogger(LogLevel minimumLogLevel, string loggerName = "ConcordiaFramework")
        {
            LoggerName = loggerName;
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            Trace(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            Trace(LogLevel.Debug, message, exception);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            Trace(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            Trace(LogLevel.Error, message, exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            Trace(LogLevel.Fatal, message);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception)
        {
            Trace(LogLevel.Fatal, message, exception);
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            Trace(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            Trace(LogLevel.Info, message, exception);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            Trace(LogLevel.Warning, message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            Trace(LogLevel.Warning, message, exception);
        }

        /// <summary>
        /// Traces the message.
        /// </summary>
        /// <param name="logLevel">The loglevel.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        private void Trace(LogLevel logLevel, string message, Exception exception = null)
        {
            if ((int)logLevel >= (int)MinimumLogLevel)
            {
                if(exception != null)
                {
                    System.Diagnostics.Trace.WriteLine($"{LoggerName} [{logLevel}]: {message}\n \t{exception.Message}");
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine($"{LoggerName} [{logLevel}]: {message}");
                }
            }
        }
    }
}
