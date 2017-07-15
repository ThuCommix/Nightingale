namespace Concordia.Framework.Sessions
{
    public enum SessionContextMode
    {
        /// <summary>
        /// Single session.
        /// </summary>
        SingleSession,

        /// <summary>
        /// Creates session for each unique thread accessing the session factory.
        /// </summary>
        SessionPerThread,

        /// <summary>
        /// Creates a session for each call.
        /// </summary>
        SessionPerCall
    }
}
