using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using Nightingale.Entities;

namespace Nightingale
{
    public interface IEntityCollection : INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>
        /// Gets the collection items.
        /// </summary>
        /// <returns>Returns the collection items.</returns>
        List<Entity> GetCollectionItems();

        /// <summary>
        /// Gets the removed collection items.
        /// </summary>
        /// <returns>Returns the removed collection items.</returns>
        List<Entity> GetRemovedCollectionItems();

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void Add(Entity entity);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Returns true when the entity was deleted.</returns>
        bool Remove(Entity entity);
    }
}
