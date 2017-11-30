using System.Collections.Generic;
using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents the interface between the <see cref="Session"/> and external logic.
    /// </summary>
    public interface ISessionPlugin
    {
        /// <summary>
        /// Commit is called when a transaction is being committed.
        /// </summary>
        void Commit();

        /// <summary>
        /// Delete is called when an entity is being deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the deletion should continue, otherwise false.</returns>
        bool Delete(Entity entity);

        /// <summary>
        /// Save is called when an entity is being saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true if the save should continue, otherwise false.</returns>
        bool Save(Entity entity);

        /// <summary>
        /// SaveChanges is called when SaveChanges on the <see cref="Session"/> is called.
        /// </summary>
        /// <param name="entities">The entities which are being saved.</param>
        /// <returns>Returns true if the save change process should continue, otherwise false.</returns>
        bool SaveChanges(IEnumerable<Entity> entities);
    }
}
