using Nightingale.Entities;

namespace Nightingale.Sessions
{
    /// <summary>
    /// Represents an interceptor for an <see cref="ISession"/>.
    /// </summary>
    public interface ISessionInterceptor
    {
        /// <summary>
        /// Gets called when an <see cref="Entity"/> is saved in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        bool Save(Entity entity);

        /// <summary>
        /// Gets called when an <see cref="Entity"/> is deleted in the <see cref="ISession"/>.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true on success.</returns>
        bool Delete(Entity entity);
    }
}
