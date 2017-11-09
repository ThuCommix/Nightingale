using Nightingale.Sessions;

namespace Nightingale.Extensions
{
    public static class SessionExtensions
    {
        /// <summary>
        /// Copies the source session cache to the target session.
        /// </summary>
        /// <param name="sourceSession">The source session.</param>
        /// <param name="targetSession">The target session.</param>
        public static void CopySessionCacheTo(this ISession sourceSession, ISession targetSession)
        {
            foreach (var entity in sourceSession.SessionCache)
            {
                targetSession.SessionCache.Insert(entity);
            }
        }

        /// <summary>
        /// Merges the session cache from the source session to the current session.
        /// </summary>
        /// <param name="targetSession">The target session.</param>
        /// <param name="sourceSession">The source session.</param>
        public static void MergeSessionCache(this ISession targetSession, ISession sourceSession)
        {
            foreach (var entity in sourceSession.SessionCache)
            {
                if (targetSession.SessionCache.Get(entity.Id, entity.GetType()) == null)
                {
                    targetSession.SessionCache.Insert(entity);
                }
            }
        }
    }
}
