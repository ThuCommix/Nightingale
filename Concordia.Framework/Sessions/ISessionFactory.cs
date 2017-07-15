using System;
using System.Collections.Generic;

namespace Concordia.Framework.Sessions
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
        /// Gets the session context.
        /// </summary>
        ISessionContext Context { get; }

        /// <summary>
        /// Gets the list of entity listeners.
        /// </summary>
        List<IEntityListener> EntityListeners { get; }

        /// <summary>
        /// Gets the list of commit listeners
        /// </summary>
        List<ICommitListener> CommitListeners { get; }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <returns>Returns a session instance.</returns>
        ISession GetCurrentSession();
    }
}
