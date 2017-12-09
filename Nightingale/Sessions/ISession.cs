using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nightingale.Entities;
using Nightingale.Queries;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a database session.
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Gets the list of session plugins.
        /// </summary>
        List<ISessionPlugin> SessionPlugins { get; }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        IConnection Connection { get; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        DeletionBehavior DeletionBehavior { get; set; }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(Entity entity);

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Save(Entity entity);

        /// <summary>
        /// Saves the changes to the database.
        /// </summary>
        /// <returns>Returns the count of updated entities.</returns>
        int SaveChanges();

        /// <summary>
        /// Discards the changes on the entities and clears the property change tracker.
        /// </summary>
        void DiscardChanges();

        /// <summary>
        /// Attaches the entity to this session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Attach(Entity entity);

        /// <summary>
        /// Detaches the entity from this session.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Detach(Entity entity);

        /// <summary>
        /// Gets the entity by the given id.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        T Get<T>(int id) where T : Entity;

        /// <summary>
        /// Gets the entity by the given id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity.</returns>
        Entity Get(int id, Type entityType);

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        Transaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable);

        /// <summary>
        /// Executes a query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of the query.</returns>
        List<Entity> ExecuteQuery(IQuery query);

        /// <summary>
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the queryable.</returns>
        IQueryable<T> Query<T>() where T : Entity;

        /// <summary>
        /// Executes the specified function and returns the result object.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The function name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns the result object.</returns>
        T ExecuteFunc<T>(string name, params QueryParameter[] parameters);

        /// <summary>
        /// Executes the specified query scalar.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="query">The query.</param>
        /// <returns>Returns the scalar result.</returns>
        T ExecuteScalar<T>(IQuery query);

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Refresh(ref Entity entity);
    }
}
