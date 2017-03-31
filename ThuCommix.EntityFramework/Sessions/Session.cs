using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Metadata;
using ThuCommix.EntityFramework.Queries;

namespace ThuCommix.EntityFramework.Sessions
{
    public abstract class Session
    {
        /// <summary>
        /// A value indicating whether calling Flush would cause database io.
        /// </summary>
        public bool IsDirty => _flushList.Any();

        /// <summary>
        /// Gets or sets the flush mode.
        /// </summary>
        public SessionFlushMode FlushMode { get; set; }

        /// <summary>
        /// A value indicating whether the debug mode is enabled.
        /// </summary>
        public bool DebugMode { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        public DeletionMode DeletionMode { get; set; }

        /// <summary>
        /// Gets the entity service.
        /// </summary>
        protected IEntityService EntityService => DependencyResolver.GetInstance<IEntityService>();

        /// <summary>
        /// Gets the entity metadata resolver.
        /// </summary>
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        /// <summary>
        /// Gets the data provider.
        /// </summary>
        protected IDataProvider DataProvider { get; }

        private readonly List<Entity> _flushList;
        private readonly string _dataProviderName;
        private bool _isInTransaction;

        /// <summary>
        /// Initializes the Session class.
        /// </summary>
        /// <param name="dataProvider">The data provider</param>
        protected Session(IDataProvider dataProvider)
        {
            if (dataProvider == null)
                throw new ArgumentNullException(nameof(dataProvider));

            DataProvider = dataProvider;
            FlushMode = SessionFlushMode.Commit;
            DeletionMode = DeletionMode.Soft;
            DebugMode = true;

            _flushList = new List<Entity>();

            var dataProviderType = dataProvider.GetType();
            _dataProviderName = ReflectionHelper.GetDisplayName(dataProviderType) ?? dataProviderType.Name;

            DataProvider.Open();
        }

        /// <summary>
        /// Deconstructs the Session class.
        /// </summary>
        ~Session()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the flush list.
        /// </summary>
        /// <returns>Returns a list of entities which are going to be saved or updated on flush.</returns>
        protected List<Entity> GetFlushList()
        {
            return _flushList;
        }

        /// <summary>
        /// Evicts an entity from the persistence cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Evict(Entity entity)
        {
            entity.Evicted = true;
            if (_flushList.Contains(entity))
                _flushList.Remove(entity);
        }

        /// <summary>
        /// Saves or updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void SaveOrUpdate(Entity entity)
        {
            var entities = EntityService.GetChildEntities(entity, Cascade.Save);

            if (entities.Any(x => x.Deleted && x.IsNotSaved))
                throw new SessionException("Insertion of a deleted entity is not allowed.");

            foreach(var entityToSave in entities)
            {
                if(entityToSave.Evicted)
                    throw new SessionException("An evicted entity can not be saved or updated. Reload the entity before performing any actions on it.");

                if (!_flushList.Contains(entityToSave))
                {
                    _flushList.Add(entityToSave);
                }
            }

            if (FlushMode == SessionFlushMode.Always)
                Flush();
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
                var constraints = ResolveDependencyConstraints(entity, entities);
                if (constraints.Count > 0)
                    throw new SessionDeleteException($"The entity '{entityToDelete.GetType().Name}' and Id = '{entityToDelete.Id}' could not be marked as deleted.", constraints);

                entityToDelete.Deleted = true;
            }

            if(DeletionMode == DeletionMode.Soft)
                SaveOrUpdate(entity);

            if (DeletionMode == DeletionMode.Hard)
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
            var query = new Query {EntityType = entityType, MaxResults = 1};
            var group = query.CreateQueryConditionGroup();
            group.CreateQueryCondition(x => x.Id == id);
            group.CreateQueryCondition(x => x.Deleted == false);

            return ExecuteQuery(query).FirstOrDefault();
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        public virtual IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (_isInTransaction)
                throw new SessionException("The session is already in a transaction.");

            _isInTransaction = true;

            return new TransactionProxy(DataProvider.BeginTransaction(isolationLevel),
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
                throw new SessionException("The session is not in a transaction.");

            _isInTransaction = false;

            DataProvider.Rollback();
        }

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint"></param>
        public virtual void RollbackTo(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.");

            DataProvider.RollbackTo(savePoint);
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void Release(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.");

            DataProvider.Release(savePoint);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public virtual void Commit()
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.");

            _isInTransaction = false;

            if (FlushMode == SessionFlushMode.Commit)
                Flush();

            DataProvider.Commit();
        }

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void Save(string savePoint)
        {
            if (!_isInTransaction)
                throw new SessionException("The session is not in a transaction.");

            DataProvider.Save(savePoint);
        }

        /// <summary>
        /// Flushes the session and writes all pending changes to the database.
        /// </summary>
        public virtual void Flush()
        {
            foreach(var entity in _flushList)
            {
                if (!entity.Validate())
                    throw new SessionException($"The entity validation failed for entity '{entity.GetType().Name}'");
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
                        $"The entity '{unsavedEntity.GetType().Name}' could not be inserted.'", ex);
                }

                if (unsavedEntity.IsNotSaved)
                {
                    throw new SessionInsertException(
                        $"The entity '{unsavedEntity.GetType().Name}' could not be inserted.'");
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
                        $"Could not update entity '{entity.GetType().Name}' and Id = '{entity.Id}'.", ex);
                }
            }

            foreach(var entity in _flushList)
            {
                entity.PropertyChangeTracker.Clear();
            }
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

            if (FlushMode == SessionFlushMode.Intelligent || FlushMode == SessionFlushMode.Always)
                Flush();

            DebugQueryResult(query);

            var dataReader = DataProvider.ExecuteReader(query);
            var entityList = new List<Entity>();

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

            dataReader.Close();
            dataReader.Dispose();

            return entityList;
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
                parameters.Add(new QueryParameter($"@{field.Name}", propertyValue));
            }

            var query = new Query(commandBuilder.ToString(), entityType, parameters);

            DebugQueryResult(query);

            return DataProvider.ExecuteInsert(query);
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

            var changedFields = entity.PropertyChangeTracker.GetChangedProperties();
            if (changedFields.Count == 0)
                return;

            commandBuilder.Append($"UPDATE {metadata.Table} SET ");
            commandBuilder.Append(string.Join(",", changedFields.Select(x => $"{x} = @{x}")));
            commandBuilder.Append($" WHERE Id = {entity.Id} AND Version = {entity.Version - 1}");

            foreach (var field in changedFields)
            {
                var propertyValue = ReflectionHelper.GetProperty(entityType, field).GetValue(entity);
                parameters.Add(new QueryParameter($"@{field}", propertyValue));
            }

            var query = new Query(commandBuilder.ToString(), entityType, parameters);

            DebugQueryResult(query);

            if (DataProvider.ExecuteNonQuery(query) == 0)
                throw new SessionUpdateException("The entity was not updated.");
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

            if (DataProvider.ExecuteNonQuery(query) == 0)
                throw new SessionDeleteException($"The entity with type '{entityType.Name}' and Id = '{entity.Id}' could not be deleted from database.");
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
                    group.CreateQueryCondition(field.Name, entity.Id);
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
        /// Emits the query command to the standard output stream.
        /// </summary>
        /// <param name="query"></param>
        private void DebugQueryResult(IQuery query)
        {
            if (!DebugMode)
                return;

            var command = query.Command;
            var parameterDebugString = string.Join(", ", query.Parameters.Select(x => x.Name + " = " + x.Value?.ToString() ?? "NULL"));

            System.Diagnostics.Debug.WriteLine($"{_dataProviderName}: {command}\nParameters: {parameterDebugString}");
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
                DataProvider.Close();
                DataProvider.Dispose();
            }
        }
    }
}
