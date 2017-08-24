using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Concordia.Framework.Entities;
using Concordia.Framework.Queries;

namespace Concordia.Framework
{
    public interface IRepository : IDisposable
    {
        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable);

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        void Rollback();

        /// <summary>
        /// Rollsbacks to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void RollbackTo(string savePoint);

        /// <summary>
        /// Creates a save point with the specified name.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        void SetSavePoint(string savePoint);

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
        /// Gets the entity specified by id and type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        T GetById<T>(int id) where T : Entity;

        /// <summary>
        /// Gets the entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        Entity GetByIdAndType(int id, Type entityType);

        /// <summary>
        /// Gets a list of entities based on the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the result list of entities.</returns>
        List<T> GetList<T>(Expression<Func<T, bool>> expression) where T : Entity;

        /// <summary>
        /// Gets a list of entities based on the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the result list of entities.</returns
        List<T> GetList<T>() where T : Entity;

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of entities.</returns>
        List<Entity> ExecuteQuery(IQuery query);

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Save(Entity entity);

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Delete(Entity entity);

        /// <summary>
        /// Executes the specified function and returns the result object.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The function name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns the result object.</returns>
        T ExecuteFunc<T>(string name, params QueryParameter[] parameters);

        /// <summary>
        /// Flushs the repository.
        /// </summary>
        void Flush();

        /// <summary>
        /// Clears the session.
        /// </summary>
        void Clear();

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Refresh(ref Entity entity);
    }
}
