using Nightingale.Entities;

namespace Nightingale
{
    public abstract class EntityListener<T> : IEntityListener where T : Entity
    {
        /// <summary>
        /// Should be called before the entity is deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        public bool Delete(Entity entity)
        {
            if(entity.GetType() == typeof(T))
            {
                return OnDelete(entity as T);
            }

            return true;
        }

        /// <summary>
        /// Should be called before the entity is saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        public bool Save(Entity entity)
        {
            if (entity.GetType() == typeof(T))
            {
                return OnSave(entity as T);
            }

            return true;
        }

        /// <summary>
        /// Should be called before the entity is deleted.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        protected abstract bool OnDelete(T entity);

        /// <summary>
        /// Should be called before the entity is saved.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        protected abstract bool OnSave(T entity);
    }
}
