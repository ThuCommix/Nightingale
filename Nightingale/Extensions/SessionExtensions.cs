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
    }
}
