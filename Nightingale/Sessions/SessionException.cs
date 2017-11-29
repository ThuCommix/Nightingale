using System;
using System.Runtime.Serialization;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents errors during <see cref="Session"/> execution.
    /// </summary>
    public class SessionException : Exception
    {
        /// <summary>
        /// Initializes a new SessionException class.
        /// </summary>
        public SessionException()
        {
        }

        /// <summary>
        /// Initializes a new SessionException class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SessionException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new SessionException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SessionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new SessionException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected SessionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
