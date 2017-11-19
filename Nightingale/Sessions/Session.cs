using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Nightingale.Caching;
using Nightingale.Entities;
using Nightingale.Extensions;
using Nightingale.Logging;
using Nightingale.Metadata;
using Nightingale.Queries;

namespace Nightingale.Sessions
{
    public class Session : ISession
    {
        /// <summary>
        /// A value indicating whether the session has a open connection.
        /// </summary>
        public bool IsOpen => Connection.IsOpen;

        /// <summary>
        /// A value indicating whether calling Flush would cause database io.
        /// </summary>
        public bool IsDirty => _flushList.Any();

        /// <summary>
        /// Gets or sets the flush mode.
        /// </summary>
        public FlushMode FlushMode { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        public DeletionMode DeletionMode { get; set; }

        /// <summary>
        /// Gets the list of entity listeners.
        /// </summary>
        public List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the list of commit listeners
        /// </summary>
        public List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// Gets the session cache.
        /// </summary>
        public ICache SessionCache => _sessionCache;

        /// <summary>
        /// Gets the entity service.
        /// </summary>
        protected IEntityService EntityService => DependencyResolver.GetInstance<IEntityService>();

        /// <summary>
        /// Gets the entity metadata resolver.
        /// </summary>
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        /// <summary>
        /// Gets the connection.
        /// </summary>
        protected IConnection Connection { get; }

        private readonly List<Entity> _flushList;
        private readonly ILogger _logger;
        private readonly ICache _sessionCache;
        private bool _isInTransaction;

        /// <summary>
        /// Initializes a new Session class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Session(IConnection connection)
        {
            if (connection == null)
                throw new ArgumentNullException(nameof(connection));

            _logger = DependencyResolver.TryGetInstance<ILogger>();
            _sessionCache = GetSessionCache();

            EntityListeners = new List<IEntityListener>();
            CommitListeners = new List<ICommitListener>();

            Connection = connection;

            _flushList = new List<Entity>();

            var connectionType = Connection.GetType();
            _logger?.Info($"Underlying database: {ReflectionHelper.GetDisplayName(connectionType) ?? connectionType.Name}");

            Connection.Open();
        }

        /// <summary>
        /// Deconstructs the Session class.
        /// </summary>
        ~Session()
        {
            Dispose(false);
        }

        /// <summary>
        /// Evicts an entity from the persistence cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Evict(Entity entity)
        {
            entity.Evicted = true;
            _flushList.Remove(entity);
            _sessionCache?.Remove(entity);
        }

        /// <summary>
        /// Saves or updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void SaveOrUpdate(Entity entity)
        {
            if (entity.Evicted)
                throw new SessionException("The entity was evicted and can not be saved.");

            var entities = EntityService.GetChildEntities(entity, Cascade.Save);

            if (entities.Any(x => x.Deleted && x.IsNotSaved))
                throw new SessionException("Insertion of a deleted entity is not allowed.").Log(_logger);

            foreach(var entityToSave in entities)
            {
                EnsureEntityListenerSave(entityToSave);

                if (!_flushList.Contains(entityToSave))
                {
                    _flushList.Add(entityToSave);
                }
            }

            if (FlushMode == FlushMode.Always)
                Flush();

            _sessionCache?.Insert(entity);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(Entity entity)
        {
            if (DeletionMode == DeletionMode.None)
                return;

            var entities = EntityService.GetChildEntities(entity, Cascade.SaveDelete);
            foreach(var entityToDelete in entities)
            {
                EnsureEntityListenerDelete(entityToDelete);

                var constraints = ResolveDependencyConstraints(entity, entities);
                if (constraints.Count > 0)
                    throw new SessionDeleteException($"The entity '{entityToDelete.GetType().Name}' and Id = '{entityToDelete.Id}' could not be marked as deleted.", constraints).Log(_logger);

                entityToDelete.Deleted = true;
            }

            if(DeletionMode == DeletionMode.Recoverable)
                SaveOrUpdate(entity);

            if (DeletionMode == DeletionMode.Irrecoverable)
            {
                foreach(var entityToDelete in entities.Where(x => x.IsSaved))
                {
                    PerformDelete(entityToDelete);
                }
            }
        }

        /// <summary>
        /// Loads an entity.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        public virtual Entity Load(int id, Type entityType)
        {
            var entity = _sessionCache?.Get(id, entityType);
            if (entity != null)
                return entity;

            return Query<Entity>().ApplyDeleteFilter().ApplyIdFilter(id).ChangeQueryType(entityType).FirstOrDefault();
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        public virtual IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (_isInTransaction)
                throw new SessionException("The session is already in a transaction.").Log(_logger);

            _isInTransaction = true;

            return new TransactionProxy(Connection.BeginTransaction(isolationLevel),
                x =>
                {
                    if (!_isInTransaction)
                        return;

                    Rollback();
                    x.Dispose();
                    _isInTransaction = false;
                });
        }

        /// <summary>
        /// Rollback the current transaction.
        /// </summary>
        public virtual void Rollback()
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.").Log(_logger);

            _isInTransaction = false;

            Connection.Rollback();
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint"></param>
        public virtual void RollbackTo(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.").Log(_logger);

            Connection.RollbackTo(savePoint);
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void Release(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.").Log(_logger);

            Connection.Release(savePoint);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public virtual void Commit()
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.").Log(_logger);

            _isInTransaction = false;

            if (FlushMode == FlushMode.Commit)
                Flush();

            CommitListeners.ForEach(x => x.Commit());
            Connection.Commit();
        }

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void Save(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.").Log(_logger);

            Connection.Save(savePoint);
        }

        /// <summary>
        /// Flushes the session and writes all pending changes to the database.
        /// </summary>
        public virtual void Flush()
        {
            foreach(var entity in _flushList)
            {
                if (entity.Evicted)
                    throw new SessionException("The entity was evicted and can not be saved.");

                if (!entity.Validate())
                    throw new SessionException($"The entity validation failed for entity '{entity.GetType().Name}'").Log(_logger);

                var entityType = entity.GetType();
                var metadata = EntityMetadataResolver.GetEntityMetadata(entity);
                foreach(var field in metadata.Fields.Where(x => x.Cascade == Cascade.None && x.IsComplexFieldType && !x.Enum))
                {
                    var referencedEntity = (Entity)entityType.GetProperty(field.GetComplexFieldName()).GetValue(entity);
                    if (referencedEntity != null && referencedEntity.IsNotSaved && !_flushList.Contains(referencedEntity))
                        throw new TransientEntityException($"Entity with type '{metadata.Name}' references an unsaved transient value in field {field.Name}. Consider saving the transient entity before flushing.").Log(_logger);
                }
            }

            foreach (var unsavedEntity in _flushList.Where(x => x.IsNotSaved))
            {
                try
                {
                    unsavedEntity.Id = PerformInsert(unsavedEntity);
                }
                catch (Exception ex)
                {
                    throw new SessionInsertException(
                        $"The entity '{unsavedEntity.GetType().Name}' could not be inserted.'", ex).Log(_logger);
                }

                if (unsavedEntity.IsNotSaved)
                {
                    throw new SessionInsertException(
                        $"The entity '{unsavedEntity.GetType().Name}' could not be inserted.'").Log(_logger);
                }
            }

            foreach (var entity in _flushList)
            {
                if (!entity.PropertyChangeTracker.HasChanges)
                    continue;

                EntityService.UpdateForeignFields(entity);

                try
                {
                    entity.Version++;
                    PerformUpdate(entity);
                }
                catch (Exception ex)
                {
                    entity.Version--;

                    throw new SessionUpdateException(
                        $"Could not update entity '{entity.GetType().Name}' and Id = '{entity.Id}'.", ex).Log(_logger);
                }
            }

            foreach(var entity in _flushList)
            {
                entity.PropertyChangeTracker.Clear();
            }

            _flushList.Clear();
        }

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of the query.</returns>
        public virtual List<Entity> ExecuteQuery(IQuery query)
        {
            if (query == null)
                throw new ArgumentNullException(nameof(query));

            if (FlushMode == FlushMode.Intelligent || FlushMode == FlushMode.Always)
                Flush();

            DebugQueryResult(query);

            var entityList = new List<Entity>();

            using (var dataReader = Connection.ExecuteReader(query))
            {
                while (dataReader.Read())
                {
                    var entity = EntityService.CreateEntity(dataReader, query.EntityType);
                    if (entity != null)
                    {
                        entity.SetSession(this);
                        entity.EagerLoadPropertiesInternal();
                        entityList.Add(entity);
                    }
                }
            }

            if (_sessionCache == null)
                return entityList;

            var persistentResults = new List<Entity>();

            foreach (var entity in entityList)
            {
                var persistentEntity = _sessionCache.Get(entity.Id, query.EntityType);
                if (persistentEntity != null)
                {
                    persistentResults.Add(persistentEntity);
                }
                else
                {
                    persistentResults.Add(entity);
                    _sessionCache.Insert(entity);
                }
            }

            return persistentResults;
        }

        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the queryable.</returns>
        public IQueryable<T> Query<T>() where T : Entity
        {
            return new Queryable<T>(this).ApplyDeleteFilter();
        }

        /// <summary>
        /// Executes the specified function and returns the result object.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The function name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns the result object.</returns>
        public virtual T ExecuteFunc<T>(string name, params QueryParameter[] parameters)
        {
            var query = new Query($"SELECT {name}({string.Join(",", parameters.Select(x => x.Name))})", null, parameters);

            return (T)Connection.ExecuteScalar(query);
        }

        /// <summary>
        /// Executes the specified query scalar.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>Returns the scalar result.</returns>
        public T ExecuteScalar<T>(IQuery query)
        {
            DebugQueryResult(query);
            return (T)Convert.ChangeType(Connection.ExecuteScalar(query), typeof(T));
        }

        /// <summary>
        /// Clears the session.
        /// </summary>
        public virtual void Clear()
        {
            _flushList.Clear();
            _sessionCache?.Clear();
        }

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Refresh(ref Entity entity)
        {
            _sessionCache.Remove(entity);
            entity = Load(entity.Id, entity.GetType());
        }

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns a table instance.</returns>
        public Table<T> GetTable<T>() where T : Entity
        {
            return Connection.GetTable<T>();
        }

        /// <summary>
        /// Gets a list of entities marked to be saved or updated.
        /// </summary>
        /// <returns>Returns a list of entities.</returns>
        protected IList<Entity> GetDirtyEntities()
        {
            return _flushList;
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the id of the entity.</returns>
        protected virtual int PerformInsert(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);
            var commandBuilder = new StringBuilder();
            var parameters = new List<QueryParameter>();

            commandBuilder.Append($"INSERT INTO {metadata.Table} (");
            var fields = metadata.Fields.Where(x => x.Name != "Id");

            commandBuilder.Append(string.Join(",", fields.Select(x => x.Name)));
            commandBuilder.Append(") VALUES (");
            commandBuilder.Append(string.Join(",", fields.Select(x => $"@{x.Name}")));
            commandBuilder.Append(");");

            foreach (var field in fields)
            {
                var propertyValue = ReflectionHelper.GetProperty(entityType, field.Name).GetValue(entity);
                parameters.Add(Queries.Query.GetQueryParameter($"@{field.Name}", propertyValue, field));
            }

            var query = new Query(commandBuilder.ToString(), entityType, parameters);

            DebugQueryResult(query);

            return Connection.ExecuteInsert(query);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void PerformUpdate(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);
            var commandBuilder = new StringBuilder();
            var parameters = new List<QueryParameter>();

            var changedFields = entity.PropertyChangeTracker.GetChangedProperties().Select(x => metadata.Fields.FirstOrDefault(y => y.Name == x)).ToList();
            if (changedFields.Count == 0)
                return;

            commandBuilder.Append($"UPDATE {metadata.Table} SET ");
            commandBuilder.Append(string.Join(",", changedFields.Select(x => $"{x.Name} = @{x.Name}")));
            commandBuilder.Append($" WHERE Id = {entity.Id} AND Version = {entity.Version - 1}");

            foreach (var field in changedFields)
            {
                var propertyValue = ReflectionHelper.GetProperty(entityType, field.Name).GetValue(entity);
                parameters.Add(Queries.Query.GetQueryParameter($"@{field.Name}", propertyValue, field));
            }

            var query = new Query(commandBuilder.ToString(), entityType, parameters);

            DebugQueryResult(query);

            if (Connection.ExecuteNonQuery(query) == 0)
                throw new SessionUpdateException("The entity was not updated.").Log(_logger);
        }

        /// <summary>
        /// Deletes the entity from the database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void PerformDelete(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);

            var query = new Query($"DELETE {metadata.Table} WHERE Id = {entity.Id} AND Version = {entity.Version}", entityType);

            if (Connection.ExecuteNonQuery(query) == 0)
                throw new SessionDeleteException($"The entity with type '{entityType.Name}' and Id = '{entity.Id}' could not be deleted from database.").Log(_logger);
        }

        /// <summary>
        /// Resolves dependency constraints.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="entitiesToDelete">The entities to delete.</param>
        /// <returns>Returns a list of contraints.</returns>
        protected virtual List<string> ResolveDependencyConstraints(Entity entity, List<Entity> entitiesToDelete)
        {
            var constraints = new List<string>();
            var entityType = entity.GetType();
            var entityMetadataList = EntityMetadataResolver.EntityMetadata.Where(x => x.Fields.Any(y => y.FieldType == entityType.Name && y.Mandatory));

            foreach (var entityMetadata in entityMetadataList)
            {
                var query = new Query { EntityType = EntityMetadataResolver.GetEntityType(entityMetadata) };
                var fields = entityMetadata.Fields.Where(x => x.FieldType == entityType.Name && x.Mandatory);

                foreach (var field in fields)
                {
                    var group = query.CreateQueryConditionGroup(QueryJunction.Or);
                    group.CreateQueryCondition(field.Name, entity.Id, System.Linq.Expressions.ExpressionType.Equal);
                }

                var results = ExecuteQuery(query).Where(x => !entitiesToDelete.Any(y => y.Id == x.Id && y.GetType() == x.GetType())).ToList();
                if (results.Count > 0)
                {
                    constraints.AddRange(results.Select(x => $"Entity '{entityMetadata.Name}' with Id = '{x.Id}' references '{entityType.Name}' in {string.Join(", ", fields.Select(y => $"'{y.Name}'"))}."));
                }
            }

            return constraints;
        }

        /// <summary>
        /// Gets the session cache.
        /// </summary>
        /// <returns>Returns the cache.</returns>
        protected virtual ICache GetSessionCache()
        {
            return new SessionCache();
        }

        /// <summary>
        /// Ensures that all entity listeners return true.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void EnsureEntityListenerSave(Entity entity)
        {
            foreach (var entityListener in EntityListeners)
            {
                if (!entityListener.Save(entity))
                    throw new SessionException($"The entity could not be saved because of entity listener {entityListener.GetType().Name}.").Log(_logger);
            }
        }

        /// <summary>
        /// Ensures that all entity listeners return true.
        /// </summary>
        /// <param name="entity">The entity.</param>
        private void EnsureEntityListenerDelete(Entity entity)
        {
            foreach (var entityListener in EntityListeners)
            {
                if (!entityListener.Delete(entity))
                    throw new SessionException($"The entity could not be deleted because of entity listener {entityListener.GetType().Name}.").Log(_logger);
            }
        }

        /// <summary>
        /// Emits the query command to the standard output stream.
        /// </summary>
        /// <param name="query"></param>
        private void DebugQueryResult(IQuery query)
        {
            var command = query.Command;
            var parameterDebugString = string.Join(", ", query.Parameters.Select(x => x.Name + " = " + (x.Value ?? "NULL")));

            _logger?.Debug($"{command}\nParameters: {parameterDebugString}");
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the session.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Flush();
                Connection.Close();
                Connection.Dispose();
            }
        }
    }
}
