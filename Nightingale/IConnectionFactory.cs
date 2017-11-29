namespace Nightingale
{
    /// <summary>
    /// Responsible for creating a <see cref="IConnection"/> based on the connection string.
    /// </summary>
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
