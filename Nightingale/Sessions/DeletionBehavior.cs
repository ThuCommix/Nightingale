namespace Nightingale.Sessions
{
    /// <summary>
    /// Controls the deletion behavir of the <see cref="Session" />.
    /// </summary>
    public enum DeletionBehavior
    {
        /// <summary>
        /// Never deletes entities.
        /// </summary>
        None,

        /// <summary>
        /// Sets the deleted flag on entities. Entities which are marked are not loaded any more but still exists in database.
        /// </summary>
        Recoverable,

        /// <summary>
        /// Removes the entities from the database.
        /// </summary>
        Irrecoverable
    }
}
