using System;
using System.Collections.Generic;

namespace Concordia.Framework.Sessions
{
    public static class SessionFactory
    {
        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static IEnumerable<Session> Sessions => _sessions;

        private static readonly List<Session> _sessions;

        /// <summary>
        /// Initializes a new SessionFactory class.
        /// </summary>
        static SessionFactory()
        {
            _sessions = new List<Session>();
        }

        /// <summary>
        /// Opens a new stateful session.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>Returns the stateful session.</returns>
        public static Session OpenSession(IDataProvider dataProvider)
        {
            var session = new StatefulSession(dataProvider);
            _sessions.Add(session);

            return session;
        }

        /// <summary>
        /// Opens a new session of the specified type.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>Returns the session.</returns>
        public static Session OpenSession<T>(IDataProvider dataProvider) where T : Session
        {
            var session = (Session)Activator.CreateInstance(typeof(T), dataProvider);
            _sessions.Add(session);

            return session;
        }
    }
}
