using System;
using System.Collections.Generic;
using Nightingale.Logging;

namespace Nightingale.Sessions
{
    public interface ISessionFactory : IDisposable
    {
        /// <summary>
        /// Gets a list of session associated with this session factory.
        /// </summary>
        IEnumerable<ISession> Sessions { get; }

        /// <summary>
        /// Gets the connection factory for this instance.
        /// </summary>
        IConnectionFactory ConnectionFactory { get; }

        /// <summary>
        /// Gets the list of entity listeners.
        /// </summary>
        List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the list of commit listeners
        /// </summary>
        List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the flush mode.
        /// </summary>
        FlushMode FlushMode { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        DeletionMode DeletionMode { get; set; }

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <returns>Returns a session instance.</returns>
        ISession OpenSession();

        /// <summary>
        /// Opens a new session.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <returns>Returns a session instance.</returns>
        ISession OpenSession<T>() where T : ISession;
    }
}
