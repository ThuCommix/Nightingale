using Concordia.Framework.Entities;

namespace Concordia.Framework.Extensions
{
    public static class TableExtensions
    {
        /// <summary>
        /// Recreates the table based on the entity metadata.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="table">The table.</param>
        public static void Recreate<T>(this Table<T> table) where T : Entity
        {
            table.Delete();
            table.Create();
        }

        /// <summary>
        /// Creates the table if it's not existant.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="table">The table.</param>
        public static void CreateIfNotExists<T>(this Table<T> table) where T : Entity
        {
            if (!table.Exists())
                table.Create();
        }
    }
}
