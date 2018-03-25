using System;
using Microsoft.AspNetCore.Http;
using Nightingale.Sessions;
using ISession = Nightingale.Sessions.ISession;

namespace Nightingale.Web
{
    /// <summary>
    /// Responsible for creating a <see cref="ISession"/> based on the connection factory and the specified settings.
    /// </summary>
    public class WebSessionFactory : SessionFactory
    {
        /// <summary>
        /// Represents the key within the http context items for a session.
        /// </summary>
        public const string SessionId = "Nightingale.SessionId";

        private static readonly object Locker = new object();
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new WebSessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        public WebSessionFactory(IConnectionFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <returns>Returns a session instance.</returns>
        public override ISession OpenSession<T>()
        {
            lock (Locker)
            {
                CleanSessions();

                if (_httpContextAccessor.HttpContext.Items.ContainsKey(SessionId))
                {
                    var existingSession = (ISession)_httpContextAccessor.HttpContext.Items[SessionId];
                    if (existingSession.Connection.IsOpen)
                    {
                        return existingSession;
                    }
                }

                var session = (T)Activator.CreateInstance(typeof(T), ConnectionFactory.CreateConnection());
                Interceptors.ForEach(session.Interceptors.Add);

                _httpContextAccessor.HttpContext.Items[SessionId] = session;

                AddSession(session);

                return session;
            }
        }
    }
}
