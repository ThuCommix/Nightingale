using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    internal class PersistenceContext : IEnumerable<Entity>
    {
        private readonly Dictionary<Type, List<Entity>> _persistentEntities;

        /// <summary>
        /// Initializes a new PersistenceContext class.
        /// </summary>
        public PersistenceContext()
        {
            _persistentEntities = new Dictionary<Type, List<Entity>>();
        }

        /// <summary>
        /// Inserts an entity in the persistence context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Insert(Entity entity)
        {
            var entityType = entity.GetType();
            if(!_persistentEntities.ContainsKey(entityType))
                _persistentEntities.Add(entityType, new List<Entity>());

            if(!_persistentEntities[entityType].Contains(entity))
                _persistentEntities[entityType].Add(entity);
        }

        /// <summary>
        /// Deletes an entity from the persistence context.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Delete(Entity entity)
        {
            var entityType = entity.GetType();
            if (!_persistentEntities.ContainsKey(entityType))
                return;

            if (_persistentEntities[entityType].Contains(entity))
                _persistentEntities[entityType].Remove(entity);
        }

        /// <summary>
        /// Performs a lookup for an entity with the specified id and given type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="type">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        public Entity Lookup(int id, Type type)
        {
            if (!_persistentEntities.ContainsKey(type))
                return null;

            return _persistentEntities[type].FirstOrDefault(x => x.Id == id);
        }

        /// <summary>
        /// Performs a lookup for an entity with the specified id and given type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        public T Lookup<T>(int id) where T : Entity
        {
            return (T) Lookup(id, typeof(T));
        }

        /// <summary>
        /// Discards all entities from the persistence context.
        /// </summary>
        public void Discard()
        {
            _persistentEntities.Clear();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<Entity> GetEnumerator()
        {
            return _persistentEntities.SelectMany(x => x.Value).GetEnumerator();
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
