﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Nightingale.Caching;
using Nightingale.Entities;
using Nightingale.Queries;

namespace Nightingale.Sessions
{
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Gets the list of entity listeners.
        /// </summary>
        List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the list of commit listeners
        /// </summary>
        List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// A value indicating whether the session has a open connection.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// A value indicating whether calling Flush would cause database io.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Gets or sets the flush mode.
        /// </summary>
        FlushMode FlushMode { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        DeletionMode DeletionMode { get; set; }

        /// <summary>
        /// Gets the session cache.
        /// </summary>
        ICache SessionCache { get; }

        /// <summary>
        /// Evicts an entity from the persistence cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Evict(Entity entity);

        /// <summary>
        /// Saves or updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SaveOrUpdate(Entity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(Entity entity);

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
        /// Rollback the current transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rollback the current transaction to the specified save point.
        /// </summary>
        /// <param name="savePoint">the save point.</param>
        void RollbackTo(string savePoint);

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Release(string savePoint);

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Creates a new save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void Save(string savePoint);

        /// <summary>
        /// Flushes the session and writes all pending changes to the database.
        /// </summary>
        void Flush();

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
        /// Clears the session.
        /// </summary>
        void Clear();

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Refresh(ref Entity entity);

        /// <summary>
        /// Gets the table.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns a table instance.</returns>
        Table<T> GetTable<T>() where T : Entity;
    }
}
