using Nightingale.Entities;

namespace Nightingale
{
    /// <summary>
    /// Provides extensions methods for <see cref="IConnection"/>.
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Creates a table if not existing.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <param name="forceCreate">A value indicating whether the table creation should be forced.</param>
        public static void CreateTable<T>(this IConnection connection, bool forceCreate = false) where T : Entity
        {
            if (forceCreate)
            {
                connection.GetTable<T>().Recreate();
            }
            else
            {
                connection.GetTable<T>().Create();
            }
        }

        /// <summary>
        /// Deletes a table if it exists.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        public static void DeleteTable<T>(this IConnection connection) where T : Entity
        {
            connection.GetTable<T>().Delete();
        }

        /// <summary>
        /// Gets a value indicating whether the table exists.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="connection">The connection.</param>
        /// <returns>Returns true when the table exists otherwise false.</returns>
        public static bool TableExists<T>(this IConnection connection) where T : Entity
        {
            return connection.GetTable<T>().Exists();
        }
    }
}
