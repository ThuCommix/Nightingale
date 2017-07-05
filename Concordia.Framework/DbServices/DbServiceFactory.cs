using Concordia.Framework.Sessions;

namespace Concordia.Framework.DbServices
{
    public static class DbServiceFactory
    {
        /// <summary>
        /// Creates the service based on the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="session">The session.</param>
        /// <returns>Returns the service proxy.</returns>
        public static T CreateService<T>(Session session) where T : IDbService
        {
            return (T)new DbServiceProxy<T>(session).GetTransparentProxy();
        }
    }
}
