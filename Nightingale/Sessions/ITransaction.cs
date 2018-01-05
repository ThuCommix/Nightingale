using System;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a database transaction.
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Raises when the transaction is committing.
        /// </summary>
        event EventHandler<EventArgs> Committing;

        /// <summary>
        /// Raises when the transaction was committed.
        /// </summary>
        event EventHandler<EventArgs> Committed;

        /// <summary>
        /// Raises when the transaction is finished.
        /// </summary>
        event EventHandler<EventArgs> Finished;

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Save(string savePoint);

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void RollbackTo(string savePoint);

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Release(string savePoint);
    }
}
