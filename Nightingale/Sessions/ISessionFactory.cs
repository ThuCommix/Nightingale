using System;
using System.Collections.Generic;
using Nightingale.Logging;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Responsible for creating a <see cref="Session" /> based on the connection factory and the specified settings.
    /// </summary>
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
        /// Gets the list of session plugins.
        /// </summary>
        List<ISessionPlugin> SessionPlugins { get; }

        /// <summary>
        /// Gets or sets the logger.
        /// </summary>
        ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the deletion mode.
        /// </summary>
        DeletionBehavior DeletionBehavior { get; set; }

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
