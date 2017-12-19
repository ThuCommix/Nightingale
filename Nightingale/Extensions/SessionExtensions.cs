using System;
using System.Threading.Tasks;
using Nightingale.Entities;
using Nightingale.Sessions;

namespace Nightingale.Extensions
{
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
    }
}
