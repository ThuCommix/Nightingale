using Nightingale.Caching;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    public class StatelessSession : Session
    {
        /// <summary>
        /// Initializes a new StatelessSession class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public StatelessSession(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Evicts an entity from the persistence cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void Evict(Entity entity)
        {
        }

        /// <summary>
        /// Saves or updates the entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public override void SaveOrUpdate(Entity entity)
        {
            base.SaveOrUpdate(entity);

            // flush if it was not flushed by SaveOrUpdate to prevent reference caching in flushlist.
            if (FlushMode != FlushMode.Always)
                Flush();
        }

        /// <summary>
        /// Gets the session cache.
        /// </summary>
        /// <returns>Returns the cache.</returns>
        protected override ICache GetSessionCache()
        {
            return null;
        }
    }
}
