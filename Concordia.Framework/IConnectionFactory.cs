namespace Concordia.Framework
{
    public interface IConnectionFactory
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        string ConnectionString { get; }

        /// <summary>
        /// Creates a new connection.
        /// </summary>
        /// <returns>Returns a connection instance.</returns>
        IConnection CreateConnection();
    }
}
