using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nightingale.Entities;
using Nightingale.Queries;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Provides extension methods for <see cref="ISession"/>.
    /// </summary>
    public static class SessionExtensions
    {
        /// <summary>
        /// Creates a new entity within the session.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="session">The session.</param>
        /// <returns>Returns a new entity.</returns>
        public static T Create<T>(this ISession session) where T : Entity, new()
        {
            var entity = new T();
            session.Save(entity);

            return entity;
        }

        /// <summary>
        /// Saves the changes to the database async.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <returns>Returns the count of updated or inserted entities.</returns>
        public static Task<int> SaveChangesAsync(this ISession session)
        {
            return Task.Run(() => session.SaveChanges());
        }

        /// <summary>
        /// Gets the entity by the given id async.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="id">The id.</param>
        /// <returns>Returns the entity or null.</returns>
        public static Task<T> GetAsync<T>(this ISession session, int id) where T : Entity
        {
            return Task.Run(() => session.Get<T>(id));
        }

        /// <summary>
        /// Gets the entity by the given id and type async.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="id">The id.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity.</returns>
        public static Task<Entity> GetAsync(this ISession session, int id, Type entityType)
        {
            return Task.Run(() => session.Get(id, entityType));
        }

        /// <summary>
        /// Deletes the entity async.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task DeleteAsync(this ISession session, Entity entity)
        {
            return Task.Run(() => session.Delete(entity));
        }

        /// <summary>
        /// Saves the entity async.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns the task instance.</returns>
        public static Task SaveAsync(this ISession session, Entity entity)
        {
            return Task.Run(() => session.Save(entity));
        }

        /// <summary>
        /// Executes a query async.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns the result list of the query.</returns>
        public static Task<List<Entity>> ExecuteQueryAsync(this ISession session, IQuery query)
        {
            return Task.Run(() => session.ExecuteQuery(query));
        }

        /// <summary>
        /// Executes the specified query scalar async.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="query">The query.</param>
        /// <returns>Returns the scalar result.</returns>
        public static Task<T> ExecuteScalarAsync<T>(this ISession session, IQuery query) where T : Entity
        {
            return Task.Run(() => session.ExecuteScalar<T>(query));
        }

        /// <summary>
        /// Executes the specified function and returns the result object async.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="session">The session.</param>
        /// <param name="name">The function name.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>Returns the result object.</returns>
        public static Task<T> ExecuteFuncAsync<T>(this ISession session, string name, params QueryParameter[] parameters) where T : Entity
        {
            return Task.Run(() => session.ExecuteFunc<T>(name, parameters));
        }
    }
}
