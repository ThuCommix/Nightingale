using Concordia.Framework.Sessions;

namespace Concordia.Framework.Web
{
    public class WebSessionFactory : SessionFactory
    {
        private static readonly object Locker = new object();

        /// <summary>
        /// Initializes a new WebSessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        public WebSessionFactory(IConnectionFactory factory) : base(factory)
        {
        }

        /// <summary>
        /// Initializes a new WebSessionFactory class.
        /// </summary>
        /// <param name="factory">The connection factory.</param>
        /// <param name="sessionContext">The session context.</param>
        public WebSessionFactory(IConnectionFactory factory, ISessionContext sessionContext) : base(factory, sessionContext)
        {
        }

        /// <summary>
        /// Gets the current session.
        /// </summary>
        /// <returns>Returns a session instance.</returns>
        public override ISession GetCurrentSession()
        {
            lock (Locker)
            {
                return base.GetCurrentSession();
            }
        }
    }
}
