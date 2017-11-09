using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Entities;

namespace Nightingale.Caching
{
    public class SessionCache : ICache
    {
        private readonly Dictionary<Type, List<Entity>> _entityCache;

        /// <summary>
        /// Initializes a new SessionCache class.
        /// </summary>
        public SessionCache()
        {
            _entityCache = new Dictionary<Type, List<Entity>>();
        }

        /// <summary>
        /// Inserts an entity into the cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Insert(Entity entity)
        {
            var entityType = entity.GetType();
            if (!_entityCache.ContainsKey(entityType))
                _entityCache.Add(entityType, new List<Entity>());

            if (!_entityCache[entityType].Contains(entity))
                _entityCache[entityType].Add(entity);
        }

        /// <summary>
        /// Gets an entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        public Entity Get(int id, Type entityType)
        {
            if (!_entityCache.ContainsKey(entityType))
                return null;

            return _entityCache[entityType].FirstOrDefault(x => x.Id == id && !x.Deleted);
        }

        /// <summary>
        /// Removes an entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        public void Remove(int id, Type entityType)
        {
            var entity = Get(id, entityType);
            if (entity != null)
                _entityCache[entityType].Remove(entity);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            foreach(var entity in _entityCache.SelectMany(x => x.Value))
            {
                entity.Evicted = true;
            }

            _entityCache.Clear();
        }

        /// <summary>
        /// Disposes the session cache.
        /// </summary>
        public void Dispose()
        {
            Clear();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _entityCache.SelectMany(x => x.Value).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
