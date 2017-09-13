using System;
using Concordia.Framework.Logging;
using log4net;
using log4net.Core;

namespace Concordia.Framework.log4net
{
    public class Logger : Logging.ILogger
    {
        /// <summary>
        /// Gets the logger name.
        /// </summary>
        public string LoggerName => _log4NetLog.Logger.Name;

        /// <summary>
        /// Gets the minimum log level.
        /// </summary>
        public LogLevel MinimumLogLevel => ConvertLog4NetLogLevel(_log4NetLog.Logger.Repository.Threshold);

        private readonly ILog _log4NetLog;

        /// <summary>
        /// Initializes a new Logger class.
        /// </summary>
        /// <param name="log">The log4net log.</param>
        public Logger(ILog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            _log4NetLog = log;
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Debug(string message)
        {
            _log4NetLog.Debug(message);
        }

        /// <summary>
        /// Logs a debug message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Debug(string message, Exception exception)
        {
            _log4NetLog.Debug(message, exception);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Error(string message)
        {
            _log4NetLog.Error(message);
        }

        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Error(string message, Exception exception)
        {
            _log4NetLog.Error(message, exception);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Fatal(string message)
        {
            _log4NetLog.Fatal(message);
        }

        /// <summary>
        /// Logs a fatal message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Fatal(string message, Exception exception)
        {
            _log4NetLog.Fatal(message, exception);
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Info(string message)
        {
            _log4NetLog.Info(message);
        }

        /// <summary>
        /// Logs an info message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Info(string message, Exception exception)
        {
            _log4NetLog.Info(message, exception);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Warn(string message)
        {
            _log4NetLog.Warn(message);
        }

        /// <summary>
        /// Logs a warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public void Warn(string message, Exception exception)
        {
            _log4NetLog.Warn(message, exception);
        }

        /// <summary>
        /// Converts the log4net log level.
        /// </summary>
        /// <param name="level">The level.</param>
        /// <returns>Returns the loglevel.</returns>
        private LogLevel ConvertLog4NetLogLevel(Level level)
        {
            switch (level.DisplayName)
            {
                case "WARN":
                    return LogLevel.Warning;

                case "NOTICE":
                    return LogLevel.Info;
            }

            return !Enum.TryParse(level.DisplayName, true, out LogLevel errorLevel)
                       ? LogLevel.Error
                       : errorLevel;
        }
    }
}
