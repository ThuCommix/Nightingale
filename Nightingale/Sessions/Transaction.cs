using System;
using System.Data;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a database transaction.
    /// </summary>
    public class Transaction : ITransaction
    {
        /// <summary>
        /// Raises when the transaction is committing.
        /// </summary>
        public event EventHandler<EventArgs> Committing;

        /// <summary>
        /// Raises when the transaction was committed.
        /// </summary>
        public event EventHandler<EventArgs> Committed;

        /// <summary>
        /// Raises when the transaction is finished.
        /// </summary>
        public event EventHandler<EventArgs> Finished; 

        private readonly IConnection _connection;
        private readonly IDbTransaction _transaction;
        private bool _isDisposed;
        private bool _isInTransaction;

        /// <summary>
        /// Initializes a new Transaction class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        public Transaction(IConnection connection, IDbTransaction transaction)
        {
            if(connection == null)
                throw new ArgumentNullException(nameof(connection));

            if(transaction == null)
                throw new ArgumentNullException(nameof(transaction));

            _connection = connection;
            _transaction = transaction;
            _isInTransaction = true;
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void Commit()
        {
            if(_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if(!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            Committing?.Invoke(this, EventArgs.Empty);
            _connection.Commit();
            Committed?.Invoke(this, EventArgs.Empty);

            FinishTransaction();
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void Rollback()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _connection.Rollback();

            FinishTransaction();
        }

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Save(string savePoint)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _connection.Save(savePoint);
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void RollbackTo(string savePoint)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _connection.RollbackTo(savePoint);
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Release(string savePoint)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(GetType().FullName);

            if (!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _connection.Release(savePoint);
        }

        /// <summary>
        /// Disposes the transaction proxy.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                if(_isInTransaction)
                    _transaction.Rollback();
            }
        }

        /// <summary>
        /// Finishs the transaction.
        /// </summary>
        private void FinishTransaction()
        {
            _transaction.Dispose();
            _isInTransaction = false;

            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}
