namespace ThuCommix.EntityFramework
{
    public interface ICommitListener
    {
        /// <summary>
        /// Called when the current transaction is being committed.
        /// </summary>
        void Commit();
    }
}
