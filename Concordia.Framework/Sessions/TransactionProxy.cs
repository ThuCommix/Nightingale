using System;

namespace Concordia.Framework.Sessions
{
    public class TransactionProxy : IDisposable
    {
        private readonly IDisposable _transaction;
        private readonly Action<IDisposable> _action;

        /// <summary>
        /// Initializes a new TransactionProxy class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="action">The action.</param>
        public TransactionProxy(IDisposable transaction, Action<IDisposable> action)
        {
            _transaction = transaction;
            _action = action;
        }

        /// <summary>
        /// Disposes the transaction proxy.
        /// </summary>
        public void Dispose()
        {
            _action.Invoke(_transaction);
        }
    }
}
