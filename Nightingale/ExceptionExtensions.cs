using System;
using Nightingale.Logging;

namespace Nightingale
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Logs the exception before returning it.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>Returns the specified exception.</returns>
        public static Exception Log(this Exception exception, ILogger logger)
        {
            logger?.Error(exception.Message);
            return exception;
        }
    }
}
