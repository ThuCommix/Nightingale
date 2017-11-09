using System;

namespace Nightingale.Sessions
{
    public interface ISessionContext : IDisposable
    {
        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <returns>Returns a session instance.</returns>
        ISession GetSession(IConnectionFactory factory);
    }
}
