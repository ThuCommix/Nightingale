namespace ThuCommix.EntityFramework.Sessions
{
    public enum DeletionMode
    {
        /// <summary>
        /// Never deletes entities.
        /// </summary>
        None,

        /// <summary>
        /// Sets the deleted flag on entities. Entities which are marked are not loaded any more but still exists in database.
        /// </summary>
        Soft,

        /// <summary>
        /// Removes the entities from the database.
        /// </summary>
        Hard
    }
}
