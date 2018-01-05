using System;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a nested transaction.
    /// </summary>
    public class NestedTransaction : ITransaction
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

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public Guid Identifier { get; }

        private readonly ITransaction _parentTransaction;
        private readonly string _savePoint;
        private bool _rollbackOnDispose;
        private bool _isInTransaction;

        /// <summary>
        /// Initializes a new NestedTransaction class.
        /// </summary>
        /// <param name="parentTransaction">The parent transaction.</param>
        public NestedTransaction(ITransaction parentTransaction)
        {
            _parentTransaction = parentTransaction;
            _rollbackOnDispose = true;
            _isInTransaction = true;

            Identifier = Guid.NewGuid();

            _savePoint = $"t_{Identifier:N}";

            parentTransaction.Save(_savePoint);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public void Commit()
        {
            if (!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _rollbackOnDispose = false;
            _isInTransaction = false;

            Committing?.Invoke(this, EventArgs.Empty);

            _parentTransaction.Release(_savePoint);

            Committed?.Invoke(this, EventArgs.Empty);
            Finished?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public void Rollback()
        {
            if(!_isInTransaction)
                throw new InvalidOperationException("The transaction was either already committed or rolled back.");

            _parentTransaction.RollbackTo(_savePoint);
            _isInTransaction = false;

            Finished?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Save(string savePoint)
        {
            throw new NotSupportedException("Saving a specific save point is not supported in a nested transaction.");
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void RollbackTo(string savePoint)
        {
            throw new NotSupportedException("Rolling back to a specific save point is not supported in a nested transaction.");
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public void Release(string savePoint)
        {
            throw new NotSupportedException("Releasing a specific save point is not supported in a nested transaction.");
        }

        /// <summary>
        /// Disposes the transaction.
        /// </summary>
        public void Dispose()
        {
            if(_rollbackOnDispose)
                Rollback();
        }
    }
}
