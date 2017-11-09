using System;
using System.Collections.Generic;
using System.Threading;
using Nightingale.Extensions;

namespace Nightingale.Sessions
{
    public class SessionContext<T> : ISessionContext where T : ISession
    {
        /// <summary>
        /// Gets or sets the session context mode.
        /// </summary>
        public SessionContextMode ContextMode { get; set; }

        private readonly Dictionary<int, ISession> _sessions;
        private ISession _singleSession;

        /// <summary>
        /// Initializes a new SessionContext class.
        /// </summary>
        public SessionContext()
        {
            _sessions = new Dictionary<int, ISession>();
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <returns>Returns a session instance.</returns>
        public ISession GetSession(IConnectionFactory factory)
        {
            if(ContextMode == SessionContextMode.SingleSession)
            {
                if (_singleSession == null || !_singleSession.IsOpen)
                    _singleSession = CreateSession(factory.CreateConnection());

                return _singleSession;
            }

            if (ContextMode == SessionContextMode.SessionPerCall)
                return CreateSession(factory.CreateConnection());

            var threadId = Thread.CurrentThread.ManagedThreadId;
            if (!_sessions.ContainsKey(threadId))
                _sessions.Add(threadId, CreateSession(factory.CreateConnection()));

            var session = _sessions[threadId];
            if(!session.IsOpen)
            {
                session = CreateSession(factory.CreateConnection());
                _sessions[threadId] = session;

            }

            return session;
        }

        /// <summary>
        /// Disposes the session context.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the session context.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _sessions.Values.ForEach(x => x.Dispose());
                _sessions.Clear();
            }
        }

        /// <summary>
        /// Creates the session.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <returns>Returns the session instance.</returns>
        private ISession CreateSession(IConnection connection)
        {
            return (ISession)Activator.CreateInstance(typeof(T), connection);
        }
    }
}
