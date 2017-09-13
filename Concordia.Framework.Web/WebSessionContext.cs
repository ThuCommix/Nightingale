using System;
using System.Collections.Generic;
using System.Linq;
using Concordia.Framework.Sessions;
using Microsoft.AspNetCore.Http;

namespace Concordia.Framework.Web
{
    public class WebSessionContext : ISessionContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Dictionary<Guid, Session> _sessions;

        /// <summary>
        /// Initializes a new WebSessionContext class.
        /// </summary>
        /// <param name="httpContextAccessor">The http context accessor.</param>
        public WebSessionContext(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _httpContextAccessor = httpContextAccessor;
            _sessions = new Dictionary<Guid, Session>();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <returns>Returns a session instance.</returns>
        public Sessions.ISession GetSession(IConnectionFactory factory)
        {
            foreach(var entry in _sessions.ToList())
            {
                if (!entry.Value?.IsOpen ?? false)
                    _sessions.Remove(entry.Key);
            }

            if(_httpContextAccessor.HttpContext.GetUniqueIdentifier() == Guid.Empty)
            {
                _httpContextAccessor.HttpContext.SetUniqueIdentifier();
            }

            var currentContext = _httpContextAccessor.HttpContext.GetUniqueIdentifier();
            if(_sessions.ContainsKey(currentContext))
            {
                var existingSession = _sessions[currentContext];
                if (existingSession.IsOpen)
                    return existingSession;
            }

            var session = new Session(factory.CreateConnection());
            _sessions[currentContext] = session;
            System.Diagnostics.Debug.WriteLine("---------------->" + currentContext.ToString());

            return session;
        }

        /// <summary>
        /// Disposes the session context.
        /// </summary>
        public void Dispose()
        {
            _sessions.Clear();
        }
    }
}
