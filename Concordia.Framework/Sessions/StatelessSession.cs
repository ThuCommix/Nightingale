using Concordia.Framework.Entities;

namespace Concordia.Framework.Sessions
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

        public override void Evict(Entity entity)
        {
        }

        public override void SaveOrUpdate(Entity entity)
        {
            base.SaveOrUpdate(entity);

            // flush if it was not flushed by SaveOrUpdate to prevent reference caching in flushlist.
            if (FlushMode != FlushMode.Always)
                Flush();
        }
    }
}
