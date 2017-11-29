using System.Collections.Generic;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents the interface between the <see cref="Session"/> and external logic.
    /// </summary>
    public abstract class SessionPluginBase<T> : ISessionPlugin where T : Entity
    {
        /// <summary>
        /// Commit is called when a transaction is being committed.
        /// </summary>
        /// <returns>Returns true if the commit should continue, otherwise false.</returns>
        public virtual bool Commit()
        {
            return true;
        }

        /// <summary>
        /// Delete is called when an entity is being deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the deletion should continue, otherwise false.</returns>
        public bool Delete(Entity entity)
        {
            if (entity is T tEntity)
                return OnDelete(tEntity);

            return true;
        }

        /// <summary>
        /// Save is called when an entity is being saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the save should continue, otherwise false.</returns>
        public bool Save(Entity entity)
        {
            if (entity is T tEntity)
                return OnSave(tEntity);

            return true;
        }

        /// <summary>
        /// SaveChanges is called when SaveChanges on the <see cref="Session"/> is called.
        /// </summary>
        /// <param name="entities">The entities which are being saved.</param>
        /// <returns>Returns true if the save change process should continue, otherwise false.</returns>
        public virtual bool SaveChanges(IEnumerable<Entity> entities)
        {
            return true;
        }

        /// <summary>
        /// OnDelete is called when an entity is being deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the deletion should continue, otherwise false.</returns>
        protected virtual bool OnDelete(T entity)
        {
            return true;
        }

        /// <summary>
        /// OnSave is called when an entity is being saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the save should continue, otherwise false.</returns>
        protected virtual bool OnSave(T entity)
        {
            return true;
        }
    }
}
