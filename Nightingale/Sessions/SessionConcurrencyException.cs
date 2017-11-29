namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a concurrency error in the <see cref="Session"/>.
    /// </summary>
    public class SessionConcurrencyException : SessionException
    {
        /// <summary>
        /// Initializes a new SessionConcurrencyException class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SessionConcurrencyException(string message) : base(message)
        {    
        }
    }
}
