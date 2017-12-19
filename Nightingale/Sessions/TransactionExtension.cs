using System.Threading.Tasks;

namespace Nightingale.Sessions
{
    public static class TransactionExtension
    {
        /// <summary>
        /// Commits the current transaction async.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task CommitAsync(this Transaction transaction)
        {
            return Task.Run(() => transaction.Commit());
        }

        /// <summary>
        /// Rollback the current transaction async.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task RollbackAsync(this Transaction transaction)
        {
            return Task.Run(() => transaction.Rollback());
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point async.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="savePoint">The save point.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task RollbackToAsync(this Transaction transaction, string savePoint)
        {
            return Task.Run(() => transaction.RollbackTo(savePoint));
        }

        /// <summary>
        /// Creates a new save point async.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="savePoint">The save point.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task SaveAsync(this Transaction transaction, string savePoint)
        {
            return Task.Run(() => transaction.Save(savePoint));
        }

        /// <summary>
        /// Releases the specified save point async.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="savePoint">The save point.</param>
        /// <returns>Returns the task instance.</returns>
        /// <returns>Returns the task instance.</returns>
        public static Task ReleaseAsync(this Transaction transaction, string savePoint)
        {
            return Task.Run(() => transaction.Release(savePoint));
        }
    }
}
