using System;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Logging;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Responsible for creating a <see cref="ISession" /> based on the connection factory and the specified settings.
    /// </summary>
    public class SessionFactory : ISessionFactory
    {
        /// <summary>
        /// Gets a list of session associated with this session factory.
        /// </summary>
        public IEnumerable<ISession> Sessions => _sessions.Where(x => x.Connection.IsOpen);

        /// <summary>
        /// Gets the connection factory for this instance.
        /// </summary>
        public IConnectionFactory ConnectionFactory { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger
        {
            get => DependencyResolver.GetInstance<ILogger>();
            set => DependencyResolver.Register(value);
        }

        /// <summary>
        /// Gets or sets the deletion behavior.
        /// </summary>
        public DeletionBehavior DeletionBehavior { get; set; }

        /// <summary>
        /// Gets the session interceptors.
        /// </summary>
        public IList<ISessionInterceptor> Interceptors { get; }

        private readonly List<ISession> _sessions;

        /// <summary>
        /// Initializes a new SessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        public SessionFactory(IConnectionFactory factory) 
        {
            ConnectionFactory = factory;
            DeletionBehavior = DeletionBehavior.Irrecoverable;
            Interceptors = new List<ISessionInterceptor>();

            _sessions = new List<ISession>();
        }

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <returns>Returns a session instance.</returns>
        public virtual ISession OpenSession()
        {
            return OpenSession<Session>();
        }

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <returns>Returns a session instance.</returns>
        public virtual ISession OpenSession<T>() where T : ISession
        {
            CleanSessions();

            var session = (T)Activator.CreateInstance(typeof(T), ConnectionFactory.CreateConnection());
            if (!_sessions.Contains(session))
            {
                AddSession(session);
                Interceptors.ForEach(x => session.Interceptors.Add(x));
            }

            session.DeletionBehavior = DeletionBehavior;

            return session;
        }

        /// <summary>
        /// Adds a new session to the list.
        /// </summary>
        /// <param name="session">The session.</param>
        protected virtual void AddSession(ISession session)
        {
            _sessions.Add(session);
        }

        /// <summary>
        /// Removes the session.
        /// </summary>
        /// <param name="session">The session.</param>
        protected virtual void RemoveSession(ISession session)
        {
            _sessions.Remove(session);
        }

        /// <summary>
        /// Cleans closed sessions.
        /// </summary>
        protected void CleanSessions()
        {
            _sessions.RemoveAll(x => !x.Connection.IsOpen);
        }

        /// <summary>
        /// Disposes the session factory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the session factory.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var session in Sessions)
                {
                    session.Dispose();
                }
            }
        }
    }
}
