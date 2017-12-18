using System.Threading.Tasks;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Provides async functionality for the <see cref="ISessionFactory"/>.
    /// </summary>
    public static class SessionFactoryExtension
    {
        /// <summary>
        /// Opens a new session async.
        /// </summary>
        /// <param name="sessionFactory">The session factory.</param>
        /// <returns>Returns a session instance.</returns>
        public static Task<ISession> OpenSessionAsync(this ISessionFactory sessionFactory)
        {
            return Task.Run(() => sessionFactory.OpenSession());
        }

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <param name="sessionFactory">The session factory.</param>
        /// <returns>Returns a session instance.</returns>
        public static Task<ISession> OpenSessionAsync<T>(this ISessionFactory sessionFactory) where T : ISession
        {
            return Task.Run(() => sessionFactory.OpenSession<T>());
        }
    }
}
