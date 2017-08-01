using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using Concordia.Framework.Sessions;

namespace Concordia.Framework.Web
{
    public class WebSessionContext : ISessionContext
    {
        private Dictionary<Guid, Session> _sessions;

        /// <summary>
        /// Initializes a new WebSessionContext class.
        /// </summary>
        public WebSessionContext()
        {
            _sessions = new Dictionary<Guid, Session>();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <returns>Returns a session instance.</returns>
        public ISession GetSession(IConnectionFactory factory)
        {
            foreach(var sessionEntry in _sessions.ToList())
            {
                if (!sessionEntry.Value?.IsOpen ?? false)
                    _sessions.Remove(sessionEntry.Key);
            }

            var currentContext = Guid.Empty;

            try
            {
                if(HttpContext.Current.GetUniqueIdentifier() == Guid.Empty)
                {
                    HttpContext.Current.SetUniqueIdentifier();
                }

                currentContext = HttpContext.Current.GetUniqueIdentifier();
            }
            catch(WebException)
            {
            }

            if(_sessions.ContainsKey(currentContext))
            {
                var existingSession = _sessions[currentContext];
                if (existingSession.IsOpen)
                    return existingSession;
            }

            var session = new StatefulSession(factory.CreateConnection());
            _sessions[currentContext] = session;

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
