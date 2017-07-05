using System.Collections.Generic;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework
{
    public interface IEntityCollection
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
    }
}
