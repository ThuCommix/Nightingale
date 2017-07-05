using System;
using System.Runtime.Serialization;

namespace Concordia.Framework.Sessions
{
    public class SessionInsertException : SessionException
    {
        public SessionInsertException()
        {
        }

        public SessionInsertException(string message) : base(message)
        {
        }

        public SessionInsertException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SessionInsertException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
