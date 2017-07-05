using System;
using System.Runtime.Serialization;

namespace Concordia.Framework.Sessions
{
    [Serializable]
    public class SessionUpdateException : Exception
    {
        public SessionUpdateException()
        {
        }

        public SessionUpdateException(string message) : base(message)
        {
        }

        public SessionUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SessionUpdateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}