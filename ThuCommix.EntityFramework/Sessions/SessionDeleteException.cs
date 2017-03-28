using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ThuCommix.EntityFramework.Sessions
{
    public class SessionDeleteException : SessionException
    {
        /// <summary>
        /// Gets the constraint violations.
        /// </summary>
        public List<string> ConstraintViolations { get; }

        public SessionDeleteException(List<string> constraintViolations)
        {
            ConstraintViolations = constraintViolations;
        }

        public SessionDeleteException(string message, List<string> constraintViolations) : base(message)
        {
            ConstraintViolations = constraintViolations;
        }

        public SessionDeleteException(string message, List<string> constraintViolations, Exception innerException) : base(message, innerException)
        {
            ConstraintViolations = constraintViolations;
        }

        protected SessionDeleteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SessionDeleteException()
        {
        }

        public SessionDeleteException(string message) : base(message)
        {
        }

        public SessionDeleteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
