using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Extensions
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
    }
}
