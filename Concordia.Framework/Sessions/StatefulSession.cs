using System;
using System.Collections.Generic;
using Concordia.Framework.Caching;
using Concordia.Framework.Entities;
using Concordia.Framework.Queries;

namespace Concordia.Framework.Sessions
{
    public class StatefulSession : Session
    {
        private readonly ICache _persistenceCache;

        /// <summary>
        /// Initializes a new StatefulSession class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public StatefulSession(IConnection connection) : base(connection)
        {
            _persistenceCache = new PersistenceCache();
        }

        /// <summary>
        /// Saves or updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void SaveOrUpdate(Entity entity)
        {
            if (entity.Evicted)
                throw new SessionException("The entity was evicted and can not be saved.");

            base.SaveOrUpdate(entity);

            _persistenceCache.Insert(entity);
        }

        /// <summary>
        /// Flushes the session and writes all pending changes to the database.
        /// </summary>
        public override void Flush()
        {
            foreach(var entity in GetFlushList())
            {
                if(entity.Evicted)
                    throw new SessionException("The entity was evicted and can not be saved.");
            }

            base.Flush();
        }

        /// <summary>
        /// Loads an entity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        public override Entity Load(int id, Type entityType)
        {
            var entity = _persistenceCache.Get(id, entityType);
            if (entity != null)
                return entity;

            return base.Load(id, entityType);
        }

        /// <summary>
        /// Evicts an entity from the persistence cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Evict(Entity entity)
        {
            base.Evict(entity);

            _persistenceCache.Remove(entity);
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of the query.</returns>
        public override List<Entity> ExecuteQuery(IQuery query)
        {
            var results = base.ExecuteQuery(query);
            var persistentResults = new List<Entity>();

            foreach(var entity in results)
            {
                var persistentEntity = _persistenceCache.Get(entity.Id, query.EntityType);
                if(persistentEntity != null)
                {
                    persistentResults.Add(persistentEntity);
                }
                else
                {
                    persistentResults.Add(entity);
                    _persistenceCache.Insert(entity);
                }
            }

            return persistentResults;
        }

        /// <summary>
        /// Clears the session.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            _persistenceCache.Clear();
        }
    }
}
