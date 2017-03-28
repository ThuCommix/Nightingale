using System;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Caching
{
    public interface ICache : IDisposable
    {
        /// <summary>
        /// Inserts an entity into the cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Insert(Entity entity);

        /// <summary>
        /// Gets an entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        Entity Get(int id, Type entityType);

        /// <summary>
        /// Removes an entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        void Remove(int id, Type entityType);

        /// <summary>
        /// Clears the cache.
        /// </summary>
        void Clear();
    }
}
