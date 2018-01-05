namespace Nightingale.Sessions
{
    internal enum InterceptorMode
    {
        /// <summary>
        /// Indicates a save interception.
        /// </summary>
        Save,

        /// <summary>
        /// Indicates a delete interception.
        /// </summary>
        Delete,

        /// <summary>
        /// Indicates a validation interception.
        /// </summary>
        Validate
    }
}
