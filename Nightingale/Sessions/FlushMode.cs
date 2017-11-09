namespace Nightingale.Sessions
{
    public enum FlushMode
    {
        /// <summary>
        /// The session is immediately flushed.
        /// </summary>
        Always,

        /// <summary>
        /// The session is flushed before any query.
        /// </summary>
        Intelligent,

        /// <summary>
        /// The session is flushed when commit is called.
        /// </summary>
        Commit,

        /// <summary>
        /// The session is only flushed when explicitly called.
        /// </summary>
        Manual
    }
}
