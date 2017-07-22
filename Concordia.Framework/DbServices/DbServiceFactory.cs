using System;
using Concordia.Framework.Sessions;

namespace Concordia.Framework.DbServices
{
    [Obsolete("The DbServices namespace with it's content will be removed in v2.0.0 to accomplish .net standard 2.0 compatibility.")]
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
