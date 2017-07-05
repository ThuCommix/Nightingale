using Concordia.Framework.Entities;

namespace Concordia.Framework.Sessions
{
    public class StatelessSession : Session
    {
        /// <summary>
        /// Initializes a new StatelessSession class.
        /// </summary>
        /// <param name="dataProvider">The data provider.</param>
        public StatelessSession(IDataProvider dataProvider) : base(dataProvider)
        {
        }

        public override void Evict(Entity entity)
        {
        }

        public override void SaveOrUpdate(Entity entity)
        {
            base.SaveOrUpdate(entity);

            // flush if it was not flushed by SaveOrUpdate to prevent reference caching in flushlist.
            if (FlushMode != SessionFlushMode.Always)
                Flush();
        }
    }
}
