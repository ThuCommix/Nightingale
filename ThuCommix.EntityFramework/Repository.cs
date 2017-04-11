using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Queries;
using ThuCommix.EntityFramework.Sessions;

namespace ThuCommix.EntityFramework
{
    public class Repository : IRepository
    {
        /// <summary>
        /// Gets the entity listeners.
        /// </summary>
        public List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the commit listeners.
        /// </summary>
        public List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// Gets the session.
        /// </summary>
        protected Session Session { get; }

        /// <summary>
        /// Initializes a new RepositoryBase class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Repository(Session session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            Session = session;
            EntityListeners = new List<IEntityListener>();
            CommitListeners = new List<ICommitListener>();
        }

        /// <summary>
        /// Deconstructs the RepositoryBase class.
        /// </summary>
        ~Repository()
        {
            Dispose(false);
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <returns>Returns an IDisposeable instance.</returns>
        public virtual IDisposable BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            return Session.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Rollbacks the current transaction.
        /// </summary>
        public virtual void Rollback()
        {
            Session.Rollback();
        }

        /// <summary>
        /// Rollsbacks to the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void RollbackTo(string savePoint)
        {
            Session.RollbackTo(savePoint);
        }

        /// <summary>
        /// Releases the specified save point.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void Release(string savePoint)
        {
            Session.Release(savePoint);
        }

        /// <summary>
        /// Creates a save point with the specified name.
        /// </summary>
        /// <param name="savePoint">The save point.</param>
        public virtual void SetSavePoint(string savePoint)
        {
            Session.Save(savePoint);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public virtual void Commit()
        {
            CommitListeners.ForEach(x => x.Commit());
            Session.Commit();
        }

        /// <summary>
        /// Flushs the repository.
        /// </summary>
        public virtual void Flush()
        {
            Session.Flush();
        }

        /// <summary>
        /// Saves the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Save(Entity entity)
        {
            if (!EntityListeners.All(x => x.Save(entity)))
                throw new InvalidOperationException("The entity could not be marked for save or update because an entity listener returned false.");

            Session.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(Entity entity)
        {
            if (!EntityListeners.All(x => x.Delete(entity)))
                throw new InvalidOperationException("The entity could not be marked for deletion because an entity listener returned false.");

            Session.Delete(entity);
        }

        /// <summary>
        /// Gets the entity specified by id and type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        public virtual T GetById<T>(int id) where T : Entity
        {
            return GetByIdAndType(id, typeof(T)) as T;
        }

        /// <summary>
        /// Gets the entity specified by id and type.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity or null.</returns>
        public virtual Entity GetByIdAndType(int id, Type entityType)
        {
            return Session.Load(id, entityType);
        }

        /// <summary>
        /// Gets a list of entities based on the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the result list of entities.</returns>
        public virtual List<T> GetList<T>(Expression<Func<T, bool>> expression = null) where T : Entity
        {
            var query = Query.CreateQuery<T>();
            var group = query.CreateQueryConditionGroup();

            group.CreateQueryCondition(x => x.Deleted == false);

            if(expression != null)
                group.CreateQueryCondition(expression);

            return Session.ExecuteQuery(query).OfType<T>().ToList();
        }

        /// <summary>
        /// Executes the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of entities.</returns>
        public virtual List<Entity> ExecuteQuery(IQuery query)
        {
            return Session.ExecuteQuery(query);
        }

        /// <summary>
        /// Executes the specified function and returns the result object.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The function name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns the result object.</returns>
        public T ExecuteFunc<T>(string name, params QueryParameter[] parameters)
        {
            return Session.ExecuteFunc<T>(name, parameters);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                Session.Dispose();
            }
        }
    }
}
