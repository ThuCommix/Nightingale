using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents a base <see cref="ISessionInterceptor"/> implementation for a specific <see cref="Entity"/>.
    /// <typeparam name="T">The entity type.</typeparam>
    /// </summary>
    public abstract class SessionInterceptor<T> : ISessionInterceptor where T : Entity
    {
        /// <summary>
        /// Gets called when an <see cref="Entity"/> is saved in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        public bool Save(Entity entity)
        {
            if (entity is T tEntity)
                return Save(tEntity);

            return true;
        }

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is deleted in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        public bool Delete(Entity entity)
        {
            if (entity is T tEntity)
                return Delete(tEntity);

            return true;
        }

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is validated in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        public bool Validate(Entity entity)
        {
            if (entity is T tEntity)
                return Validate(tEntity);

            return true;
        }

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is saved in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        protected virtual bool Save(T entity)
        {
            return true;
        }

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is deleted in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        protected virtual bool Delete(T entity)
        {
            return true;
        }

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is validated in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        protected virtual bool Validate(T entity)
        {
            return true;
        }
    }
}
