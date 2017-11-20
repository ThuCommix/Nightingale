using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Nightingale.Sessions;
using ISession = Nightingale.Sessions.ISession;

namespace Nightingale.Web
{
    public class WebSessionFactory : SessionFactory
    {
        private static readonly object Locker = new object();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Guid, ISession> _sessions;

        /// <summary>
        /// Initializes a new WebSessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        public WebSessionFactory(IConnectionFactory factory, IHttpContextAccessor httpContextAccessor) : base(factory)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessions = new Dictionary<Guid, ISession>();
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
                foreach (var entry in _sessions.ToList())
                {
                    if (!entry.Value?.IsOpen ?? false)
                        _sessions.Remove(entry.Key);
                }

                if (_httpContextAccessor.HttpContext.GetUniqueIdentifier() == Guid.Empty)
                {
                    _httpContextAccessor.HttpContext.SetUniqueIdentifier();
                }

                var currentContext = _httpContextAccessor.HttpContext.GetUniqueIdentifier();
                if (_sessions.ContainsKey(currentContext))
                {
                    var existingSession = _sessions[currentContext];
                    if (existingSession.IsOpen)
                        return existingSession;
                }

                var session = (T)Activator.CreateInstance(typeof(T), ConnectionFactory.CreateConnection());
                _sessions[currentContext] = session;

                return session;
            }
        }

        /// <summary>
        /// Disposes the session factory.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _sessions.Clear();
        }
    }
}
