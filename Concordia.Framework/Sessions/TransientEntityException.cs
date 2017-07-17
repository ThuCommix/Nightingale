using System;
using System.Runtime.Serialization;

namespace Concordia.Framework.Sessions
{
    public class TransientEntityException : SessionException
    {
        /// <summary>
        /// Initializes a new TransientEntityException class.
        /// </summary>
        public TransientEntityException()
        {
        }

        /// <summary>
        /// Initializes a new TransientEntityException class.
        /// </summary>
        /// <param name="message">The message.</param>
        public TransientEntityException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new TransientEntityException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public TransientEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new TransientEntityException class.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context.</param>
        protected TransientEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
