using System;

namespace ThuCommix.EntityFramework.Sessions
{
    public static class SessionFactory
    {
        /// <summary>
        /// Gets the current session.
        /// </summary>
        public static Session CurrentSession { get; private set; }

        /// <summary>
        /// Opens a new stateful session.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>Returns the stateful session.</returns>
        public static Session OpenSession(IDataProvider dataProvider)
        {
            CurrentSession = new StatefulSession(dataProvider);
            return CurrentSession;
        }

        /// <summary>
        /// Opens a new session of the specified type.
        /// </summary>
        /// <typeparam name="T">The session type.</typeparam>
        /// <param name="dataProvider">The data provider.</param>
        /// <returns>Returns the session.</returns>
        public static Session OpenSession<T>(IDataProvider dataProvider) where T : Session
        {
            CurrentSession = (Session)Activator.CreateInstance(typeof(T), dataProvider);
            return CurrentSession;
        }
    }
}
