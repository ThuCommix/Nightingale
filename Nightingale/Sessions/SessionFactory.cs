using System;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Logging;

namespace Nightingale.Sessions
{
    public class SessionFactory : ISessionFactory
    {
        /// <summary>
        /// Gets a list of session associated with this session factory.
        /// </summary>
        public IEnumerable<ISession> Sessions => _sessions.Where(x => x.IsOpen);

        /// <summary>
        /// Gets the connection factory for this instance.
        /// </summary>
        public IConnectionFactory ConnectionFactory { get; }

        /// <summary>
        /// Gets the list of entity listeners.
        /// </summary>
        public List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the list of commit listeners
        /// </summary>
        public List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        public ILogger Logger
        {
            get => DependencyResolver.GetInstance<ILogger>();
            set => DependencyResolver.Register(value);
        }

        /// <summary>
        /// Gets or sets the flush mode.
        /// </summary>
        public FlushMode FlushMode { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        public DeletionMode DeletionMode { get; set; }

        private readonly List<ISession> _sessions;

        /// <summary>
        /// Initializes a new SessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        public SessionFactory(IConnectionFactory factory) 
        {
            ConnectionFactory = factory;

            EntityListeners = new List<IEntityListener>();
            CommitListeners = new List<ICommitListener>();
            _sessions = new List<ISession>();

            FlushMode = FlushMode.Commit;
            DeletionMode = DeletionMode.Recoverable;
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
                session.EntityListeners.AddRange(EntityListeners);
                session.CommitListeners.AddRange(CommitListeners);
            }

            session.FlushMode = FlushMode;
            session.DeletionMode = DeletionMode;

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
            _sessions.RemoveAll(x => !x.IsOpen);
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
