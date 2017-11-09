using Nightingale.Entities;

namespace Nightingale
{
    public interface IEntityListener
    {
        /// <summary>
        /// Should be called before the entity is saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        bool Save(Entity entity);

        /// <summary>
        /// Should be called before the entity is deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        bool Delete(Entity entity);
    }
}
