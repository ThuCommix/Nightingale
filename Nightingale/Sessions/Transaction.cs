using System;
using System.Data;

namespace Nightingale.Sessions
{
    public class Transaction : IDisposable
    {
        private readonly ISession _session;
        private readonly IDbTransaction _transaction;
        private readonly Action<IDbTransaction> _disposeCallback;

        /// <summary>
        /// Initializes a new Transaction class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="disposeCallback">The dispose callback.</param>
        public Transaction(ISession session, IDbTransaction transaction, Action<IDbTransaction> disposeCallback)
        {
            if(session == null)
                throw new ArgumentNullException(nameof(session));

            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            if(disposeCallback == null)
                throw new ArgumentNullException(nameof(disposeCallback));

            _session = session;
            _transaction = transaction;
            _disposeCallback = disposeCallback;
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void Commit()
        {
            _session.Commit();
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void Rollback()
        {
            _session.Rollback();
        }

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Save(string savePoint)
        {
            _session.Save(savePoint);
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint">the save point.</param>
        public void RollbackTo(string savePoint)
        {
            _session.RollbackTo(savePoint);
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Release(string savePoint)
        {
            _session.Release(savePoint);
        }

        /// <summary>
        /// Disposes the transaction proxy.
        /// </summary>
        public void Dispose()
        {
            _disposeCallback.Invoke(_transaction);
        }
    }
}
