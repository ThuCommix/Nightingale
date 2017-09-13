using System;
using System.Collections.Generic;
using System.Linq;
using Concordia.Framework.Logging;

namespace Concordia.Framework.Sessions
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
        /// Gets the session context.
        /// </summary>
        public ISessionContext Context { get; }

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
            : this(factory, new SessionContext<Session> { ContextMode = SessionContextMode.SessionPerThread })
        {
        }

        /// <summary>
        /// Initializes a new SessionFactory class.
        /// </summary>
        /// <param name="sessionContext">The session context.</param>
        /// <param name="factory">The connection factory.</param>
        public SessionFactory(IConnectionFactory factory, ISessionContext sessionContext)
        {
            Context = sessionContext;
            ConnectionFactory = factory;

            EntityListeners = new List<IEntityListener>();
            CommitListeners = new List<ICommitListener>();
            _sessions = new List<ISession>();

            FlushMode = FlushMode.Commit;
            DeletionMode = DeletionMode.Recoverable;
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <returns>Returns a session instance.</returns>
        public virtual ISession GetCurrentSession()
        {
            _sessions.RemoveAll(x => !x.IsOpen);

            var session = Context.GetSession(ConnectionFactory);
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
        /// Disposes the session factory.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Adds the specified session to the session list.
        /// </summary>
        /// <param name="session">The session.</param>
        protected void AddSession(ISession session)
        {
            _sessions.Add(session);
        }

        /// <summary>
        /// Removes the specified session from the session list.
        /// </summary>
        /// <param name="session">The session.</param>
        protected void RemoveSession(ISession session)
        {
            _sessions.Remove(session);
        }

        /// <summary>
        /// Disposes the session factory.
        /// </summary>
        /// <param name="disposing">The disposing state.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Context.Dispose();
            }
        }
    }
}
