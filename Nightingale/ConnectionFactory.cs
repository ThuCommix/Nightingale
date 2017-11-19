using System;
using Nightingale.Queries;

namespace Nightingale
{
    public abstract class ConnectionFactory<T> : IConnectionFactory where T : IConnection
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString { get; protected set; }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns>Returns a connection instance.</returns>
        public virtual IConnection CreateConnection()
        {
            return (IConnection)Activator.CreateInstance(typeof(T), ConnectionString);
        }

        /// <summary>
        /// Applies the sql engine.
        /// </summary>
        /// <param name="sqlEngine">The sql engine.</param>
        protected void ApplySqlEngine(ISqlGenerator sqlEngine)
        {
            SqlGeneratorManager.SqlEngine = sqlEngine;
        }
    }
}
