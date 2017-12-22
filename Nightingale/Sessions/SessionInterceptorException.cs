using System;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a <see cref="SessionException"/> which occurs when a <see cref="ISessionInterceptor"/> has returned false on either <see cref="ISessionInterceptor.Save"/> or <see cref="ISessionInterceptor.Delete"/>.
    /// </summary>
    public class SessionInterceptorException : SessionException
    {
        /// <summary>
        /// Gets the interceptor type.
        /// </summary>
        public Type InterceptorType { get; }

        /// <summary>
        /// Initializes a new SessionInterceptorException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="interceptorType">The interceptor type.</param>
        public SessionInterceptorException(string message, Type interceptorType) : base(message)
        {
            InterceptorType = interceptorType;
        }
    }
}
