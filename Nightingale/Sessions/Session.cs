using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nightingale.Entities;
using Nightingale.Extensions;
using Nightingale.Logging;
using Nightingale.Metadata;
using Nightingale.Queries;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a database session.
    /// </summary>
    public class Session : ISession
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        public IConnection Connection { get; }

        /// <summary>
        /// Gets or sets the deletion behavior.
        /// </summary>
        public DeletionBehavior DeletionBehavior { get; set; }

        /// <summary>
        /// Gets the session interceptors.
        /// </summary>
        public IList<ISessionInterceptor> Interceptors { get; }

        /// <summary>
        /// Gets the entity service.
        /// </summary>
        protected IEntityService EntityService => DependencyResolver.GetInstance<IEntityService>();

        /// <summary>
        /// Gets the entity metadata resolver.
        /// </summary>
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        private readonly ILogger _logger;
        private readonly PersistenceContext _persistenceContext;
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
            _persistenceContext = new PersistenceContext();

            Interceptors = new List<ISessionInterceptor>();
            DeletionBehavior = DeletionBehavior.Irrecoverable;
            Connection = connection;
        }

        /// <summary>
        /// Deconstructs the Session class.
        /// </summary>
        ~Session()
        {
            Dispose(false);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(Entity entity)
        {
            if(entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (DeletionBehavior == DeletionBehavior.None)
                return;

            var entities = EntityService.GetChildEntities(entity, Cascade.SaveDelete);
            entities.ForEach(x => Intercept(x, InterceptorMode.Delete));

            foreach(var entityToDelete in entities)
            {
                entityToDelete.Deleted = true;
            }
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Save(Entity entity)
        {
            if(entity.IsNotSaved && entity.Deleted)
                throw new InvalidOperationException("Insertion of a deleted entity is not allowed.");

            Attach(entity);

            var entities = EntityService.GetChildEntities(entity, Cascade.Save);
            entities.ForEach(_persistenceContext.Insert);
            entities.ForEach(x => Intercept(x, InterceptorMode.Save));
        }

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        /// <returns>Returns the count of updated entities.</returns>
        public virtual int SaveChanges()
        {
            var changedEntityList = new List<Entity>();
            var persistentEntities = _persistenceContext.ToList();

            foreach (var entity in persistentEntities.Where(x => !x.Deleted))
            {
                if (!entity.Validate())
                    throw new SessionException($"The entity validation failed for entity '{entity.GetType().Name}'").Log(_logger);

                var entityType = entity.GetType();
                var metadata = EntityMetadataResolver.GetEntityMetadata(entity);

                foreach (var field in metadata.Fields.Where(x => x.Cascade == Cascade.None && x.IsComplexFieldType && !x.Enum))
                {
                    var referencedEntity = (Entity)entityType.GetProperty(field.GetComplexFieldName()).GetValue(entity);
                    if (referencedEntity != null && referencedEntity.IsNotSaved && !persistentEntities.Contains(referencedEntity))
                        throw new TransientEntityException($"Entity with type '{metadata.Name}' references an unsaved transient value in field {field.Name}. Consider saving the transient entity before calling SaveChanges.").Log(_logger);

                    if(referencedEntity != null && field.Mandatory && entity.Deleted)
                        throw new InvalidOperationException($"Entity with type '{metadata.Name}' references an entity which is marked as deleted.");
                }
            }

            // handle deletion
            foreach (var entity in persistentEntities.Where(x => x.Deleted))
            {
                var constraints = ResolveDependencyConstraints(entity, persistentEntities);
                if (constraints.Count > 0)
                    throw new SessionDeleteException($"The entity '{entity.GetType().Name}' and Id = '{entity.Id}' could not be marked as deleted.", constraints).Log(_logger);

                if (DeletionBehavior == DeletionBehavior.Irrecoverable)
                {
                    if (entity.IsSaved)
                    {
                        DeleteEntity(entity);
                        _persistenceContext.Delete(entity);

                        changedEntityList.Add(entity);
                    }
                }
            }

            persistentEntities = _persistenceContext.ToList();

            foreach (var unsavedEntity in persistentEntities.Where(x => x.IsNotSaved))
            {
                try
                {
                    unsavedEntity.Id = InsertEntity(unsavedEntity);
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

                changedEntityList.Add(unsavedEntity);
            }

            foreach (var entity in persistentEntities)
            {
                if (!entity.PropertyChangeTracker.HasChanges)
                    continue;

                EntityService.UpdateForeignFields(entity);

                if (!ValidateEntityVersion(entity))
                    throw new SessionConcurrencyException($"The entity with type'{entity.GetType().Name}' and Id = '{entity.Id}' was modified in database.");

                try
                {
                    entity.Version++;
                    UpdateEntity(entity);

                    if(!changedEntityList.Contains(entity))
                        changedEntityList.Add(entity);
                }
                catch (Exception ex)
                {
                    entity.Version--;

                    throw new SessionUpdateException(
                        $"Could not update entity '{entity.GetType().Name}' and Id = '{entity.Id}'.", ex).Log(_logger);
                }
            }

            foreach (var entity in persistentEntities)
            {
                entity.PropertyChangeTracker.Clear();
                if(entity.Deleted)
                    _persistenceContext.Delete(entity);
            }

            return changedEntityList.Count;
        }

        /// <summary>
        /// Discards the changes on the entities and clears the property change tracker.
        /// </summary>
        public virtual void DiscardChanges()
        {
            foreach (var entity in _persistenceContext)
            {
                if(!entity.PropertyChangeTracker.HasChanges)
                    continue;

                entity.PropertyChangeTracker.DisableChangeTracking = true;

                var entityType = entity.GetType();
                var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);
                foreach (var field in metadata.Fields)
                {
                    if (entity.PropertyChangeTracker.TryGetReplacedValue(field.GetComplexFieldName(), out object propertyValue))
                    {
                        entityType.GetProperty(field.GetComplexFieldName()).SetValue(entity, propertyValue);
                    }
                }

                foreach (var listField in metadata.ListFields)
                {
                    var list = entityType.GetProperty(listField.Name).GetValue(entity) as IEntityCollection;

                    if (entity.PropertyChangeTracker.TryGetChangedCollectionItems(listField.Name, CollectionChangeType.Added, out List<Entity> addedItems))
                    {
                        addedItems.ForEach(e => list.Remove(e));
                    }

                    if (entity.PropertyChangeTracker.TryGetChangedCollectionItems(listField.Name, CollectionChangeType.Removed, out List<Entity> removedItems))
                    {
                        removedItems.ForEach(list.Add);
                    }
                }

                entity.PropertyChangeTracker.Clear();
                entity.PropertyChangeTracker.DisableChangeTracking = false;
            }
        }

        /// <summary>
        /// Attaches the entity to this session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Attach(Entity entity)
        {
            entity.SetSession(this);
            _persistenceContext.Insert(entity);
        }

        /// <summary>
        /// Detaches the entity from this session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Detach(Entity entity)
        {
            entity.SetSession(null);
            _persistenceContext.Delete(entity);
        }

        /// <summary>
        /// Gets the entity by the given id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity.</returns>
        public virtual Entity Get(int id, Type entityType)
        {
            var entity = _persistenceContext.Lookup(id, entityType);
            if (entity != null)
                return entity;

            return Query<Entity>().ApplyDeleteFilter().ApplyIdFilter(id).ChangeQueryType(entityType).FirstOrDefault();
        }

        /// <summary>
        /// Gets the entity by the given id.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        public virtual T Get<T>(int id) where T : Entity
        {
            return (T) Get(id, typeof(T));
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        public virtual Transaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            if (_isInTransaction)
                throw new SessionException("The session is already in a transaction.").Log(_logger);

            _isInTransaction = true;

            var transaction = new Transaction(Connection, Connection.BeginTransaction(isolationLevel));

            transaction.Finished += (sender, args) =>
            {
                _isInTransaction = false;
            };

            return transaction;
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

            var persistentResults = new List<Entity>();
            foreach (var entity in entityList)
            {
                var existingEntity = _persistenceContext.Lookup(entity.Id, query.EntityType);
                if (existingEntity == null)
                {
                    persistentResults.Add(entity);
                    _persistenceContext.Insert(entity);
                }
                else
                {
                    persistentResults.Add(existingEntity);
                }
            }

            return persistentResults;
        }

        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the queryable.</returns>
        public virtual IQueryable<T> Query<T>() where T : Entity
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
            var result = Connection.ExecuteScalar(query);
            if (result == null)
                return default(T);

            var underlyingType = Nullable.GetUnderlyingType(typeof(T));
            return (T)Convert.ChangeType(result, underlyingType ?? typeof(T));
        }

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Refresh(ref Entity entity)
        {
            _persistenceContext.Delete(entity);
            entity = Get(entity.Id, entity.GetType());
        }

        /// <summary>
        /// Inserts the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the id of the entity.</returns>
        protected virtual int InsertEntity(Entity entity)
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
                parameters.Add(QueryHelper.GetQueryParameter($"@{field.Name}", propertyValue, field));
            }

            var query = new Query(commandBuilder.ToString(), entityType, parameters);

            DebugQueryResult(query);

            return Connection.ExecuteInsert(query);
        }

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        protected virtual void UpdateEntity(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);
            var commandBuilder = new StringBuilder();
            var parameters = new List<QueryParameter>();

            var changedProperties = entity.PropertyChangeTracker.GetChangedProperties();
            if (!changedProperties.Any())
                return;

            commandBuilder.Append($"UPDATE {metadata.Table} SET ");
            commandBuilder.Append(string.Join(",", changedProperties.Select(x => $"{x} = @{x}")));
            commandBuilder.Append($" WHERE Id = {entity.Id} AND Version = {entity.Version - 1}");

            foreach (var propertyName in changedProperties)
            {
                var field = metadata.Fields.FirstOrDefault(x => x.Name == propertyName);
                var propertyValue = ReflectionHelper.GetProperty(entityType, field.Name).GetValue(entity);
                parameters.Add(QueryHelper.GetQueryParameter($"@{field.Name}", propertyValue, field));
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
        protected virtual void DeleteEntity(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);

            var query = new Query($"DELETE FROM {metadata.Table} WHERE Id = {entity.Id} AND Version = {entity.Version}", entityType);

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
                var currentType = EntityMetadataResolver.GetEntityType(entityMetadata);
                var fields = entityMetadata.Fields.Where(x => x.FieldType == entityType.Name && x.Mandatory);

                var parameter = Expression.Parameter(currentType, "x");
                Expression resultExpression = null;

                foreach (var field in fields)
                {
                    var body = Expression.Equal(Expression.Property(parameter, field.Name), Expression.Constant(entity.Id));
                    resultExpression = resultExpression == null ? body : Expression.OrElse(resultExpression, body);
                }

                var finalExpression = Expression.Lambda(resultExpression, parameter);
                var queryable = new QueryProvider(this).CreateQuery(Expression.Constant(null, typeof(IQueryable<>).MakeGenericType(currentType)));
                var methodInfo = typeof(Queryable).GetMethods().FirstOrDefault(x => x.Name == "Where").MakeGenericMethod(currentType);
                var methodCall = Expression.Call(methodInfo, queryable.Expression, finalExpression);

                queryable = queryable.Provider.CreateQuery(methodCall);

                var results = queryable.Cast<Entity>().ChangeQueryType(currentType).ToList().Except(entitiesToDelete).ToList();
                if (results.Count > 0)
                {
                    constraints.AddRange(results.Select(x => $"Entity '{entityMetadata.Name}' with Id = '{x.Id}' references '{entityType.Name}' in {string.Join(", ", fields.Select(y => $"'{y.Name}'"))}."));
                }
            }

            return constraints;
        }

        /// <summary>
        /// Intercepts a session functionality.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="interceptorMode">The interceptor mode.</param>
        private void Intercept(Entity entity, InterceptorMode interceptorMode)
        {
            if (interceptorMode == InterceptorMode.Save)
            {
                foreach (var interceptor in Interceptors)
                {
                    if (!interceptor.Save(entity))
                        throw new SessionInterceptorException($"The entity with type '{entity.GetType().Name}' and Id = '{entity.Id}' could not be saved.", interceptor.GetType())
                            .Log(_logger);
                }
            }

            if (interceptorMode == InterceptorMode.Delete)
            {
                foreach (var interceptor in Interceptors)
                {
                    if (!interceptor.Delete(entity))
                        throw new SessionInterceptorException($"The entity with type '{entity.GetType().Name}' and Id = '{entity.Id}' could not be deleted.", interceptor.GetType())
                            .Log(_logger);
                }
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
        /// Validates the entity version.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="metadata">The entity metadata.</param>
        /// <returns>Returns true if the database has the same version of the entity as the session.</returns>
        private bool ValidateEntityVersion(Entity entity, EntityMetadata metadata = null)
        {
            if (metadata == null)
                metadata = EntityMetadataResolver.GetEntityMetadata(entity);

            var query = new Query {Command = $"SELECT Version FROM {metadata.Table} WHERE Id = @p0"};
            query.Parameters.Add(new QueryParameter("@p0", entity.Id, SqlDbType.Int, false));

            var result = ExecuteScalar<long?>(query);
            return result == null || result == entity.Version;
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
                Connection.Close();
                Connection.Dispose();
            }
        }
    }
}
