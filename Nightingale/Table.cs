using System;
using Nightingale.Entities;
using Nightingale.Metadata;
using Nightingale.Queries;

namespace Nightingale
{
    /// <summary>
    /// Represents a database table and can perform basic operations.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public abstract class Table<T> where T : Entity
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        public Type Type => typeof(T);

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        protected EntityMetadata Metadata => DependencyResolver.GetInstance<IEntityMetadataResolver>().GetEntityMetadata(Type);

        /// <summary>
        /// Gets the connection.
        /// </summary>
        protected IConnection Connection { get; }

        /// <summary>
        /// Initializes a new Table class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        protected Table(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            Connection = connection;
        }

        /// <summary>
        /// Creates the table.
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Deletes the table.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Gets a value indicating whether the table exists.
        /// </summary>
        public abstract bool Exists();

        /// <summary>
        /// Executes the query without results.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the count of affected rows.</returns>
        public virtual int ExecuteNonQuery(IQuery query)
        {
            return Connection.ExecuteNonQuery(query);
        }
    }
}
