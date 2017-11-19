using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Nightingale.Entities;
using Nightingale.Queries;
using Nightingale.Sessions;

namespace Nightingale
{
    public class Repository : IRepository
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        protected ISession Session { get; }

        /// <summary>
        /// Gets the entity service.
        /// </summary>
        protected IEntityService EntityService => DependencyResolver.GetInstance<IEntityService>();

        /// <summary>
        /// Initializes a new RepositoryBase class.
        /// </summary>
        /// <param name="session">The session.</param>
        public Repository(ISession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            Session = session;
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
            Session.SaveOrUpdate(entity);
        }

        /// <summary>
        /// Deletes the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void Delete(Entity entity)
        {
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
            return (T) GetByIdAndType(id, typeof(T));
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
        public virtual List<T> GetList<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            if(expression == null)
                throw new ArgumentNullException(nameof(expression));

            return Query<T>().Where(expression).ToList();
        }


        /// <summary>
        /// Gets a list of entities based on the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the result list of entities.</returns>
        public virtual List<T> GetList<T>() where T : Entity
        {
            return Query<T>().ToList();
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
        /// Gets the queryable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns the queryable.</returns>
        public IQueryable<T> Query<T>() where T : Entity
        {
            return Session.Query<T>();
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
        /// Clears the session.
        /// </summary>
        public void Clear()
        {
            Session.Clear();
        }

        /// <summary>
        /// Refreshs the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void Refresh(ref Entity entity)
        {
            Session.Refresh(ref entity);
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
