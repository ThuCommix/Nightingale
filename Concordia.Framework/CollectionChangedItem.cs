using Concordia.Framework.Entities;

namespace Concordia.Framework
{
    public class CollectionChangedItem
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public Entity Item { get; }

        /// <summary>
        /// Gets the collection change type.
        /// </summary>
        public CollectionChangeType CollectionChangeType { get; }

        /// <summary>
        /// Initializes a new CollectionChange
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="item">The entity item.</param>
        /// <param name="changedType">The collection changed type.</param>
        public CollectionChangedItem(string propertyName, Entity item, CollectionChangeType changeType)
        {
            PropertyName = propertyName;
            Item = item;
            CollectionChangeType = changeType;
        }
    }
}
