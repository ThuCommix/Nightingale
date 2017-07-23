using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Concordia.Framework.Entities;
using Concordia.Framework.Metadata;

namespace Concordia.Framework
{
    public class PropertyChangeTracker
    {
        /// <summary>
        /// A value indicating whether change tracking is disabled.
        /// </summary>
        public bool DisableChangeTracking { get; set; }

        /// <summary>
        /// A value indicating whether the entity has changes.
        /// </summary>
        public bool HasChanges
        {
            get
            {
                var propertyChangedGroups = _propertyChangedItems.GroupBy(x => x.PropertyName);
                return propertyChangedGroups.Any(propertyChangedGroup => HasChanged(propertyChangedGroup.Key));
            }
        }

        private IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        private readonly List<CollectionChangedItem> _collectionChangedItems;
        private readonly List<PropertyChangedItem> _propertyChangedItems;
        private readonly Entity _entity;

        /// <summary>
        /// Initializes a new PropertyChangeTracker class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public PropertyChangeTracker(Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _entity = entity;
            _propertyChangedItems = new List<PropertyChangedItem>();
            _collectionChangedItems = new List<CollectionChangedItem>();
        }
        
        /// <summary>
        /// Clears the property change tracker.
        /// </summary>
        public void Clear()
        {
            _propertyChangedItems.Clear();
            _collectionChangedItems.Clear();
        }

        /// <summary>
        /// Adds a new property changed item.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="propertyExpression">The expression representing the local property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public void AddPropertyChangedItem<T>(Expression<Func<T, object>> propertyExpression, object oldValue, object newValue) where T : Entity
        {
            AddPropertyChangedItem(GetPropertyName(propertyExpression), oldValue, newValue);
        }

        /// <summary>
        /// Adds a new property changed item.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public void AddPropertyChangedItem(string property, object oldValue, object newValue)
        {
            AddPropertyChangedItem(new PropertyChangedItem(property, oldValue, newValue));
        }

        /// <summary>
        /// Adds a new property changed item.
        /// </summary>
        /// <param name="propertyChangedItem">The property changed item.</param>
        public void AddPropertyChangedItem(PropertyChangedItem propertyChangedItem)
        {
            if (DisableChangeTracking)
                return;

            _propertyChangedItems.Add(propertyChangedItem);
        }

        /// <summary>
        /// Adds a new collection changed item.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="propertyExpression">The expression representing the local property.</param>
        /// <param name="entityItem">The added or removed entity.</param>
        /// <param name="changeType">The collection change type.</param>
        public void AddCollectionChangedItem<T>(Expression<Func<T, object>> propertyExpression, Entity entityItem, CollectionChangeType changeType) where T : Entity
        {
            AddCollectionChangedItem(GetPropertyName(propertyExpression), entityItem, changeType);
        }

        /// <summary>
        /// Adds a new collection changed item.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="entityItem">The added or removed entity.</param>
        /// <param name="changeType">The collection change type.</param>
        public void AddCollectionChangedItem(string property, Entity entityItem, CollectionChangeType changeType)
        {
            AddCollectionChangedItem(new CollectionChangedItem(property, entityItem, changeType));
        }

        /// <summary>
        /// Adds a new collection changed item.
        /// </summary>
        /// <param name="collectionChangedItem">The collection changed item.</param>
        public void AddCollectionChangedItem(CollectionChangedItem collectionChangedItem)
        {
            if (DisableChangeTracking)
                return;

            var oppositeChangeType = collectionChangedItem.CollectionChangeType == CollectionChangeType.Added ? CollectionChangeType.Removed : CollectionChangeType.Added;

            // check if the item was removed previously
            var changedItem = _collectionChangedItems.FirstOrDefault(
                x => x.Item == collectionChangedItem.Item && x.PropertyName == collectionChangedItem.PropertyName && x.CollectionChangeType == oppositeChangeType);

            if (changedItem != null)
            {
                // delete the changed item and cancel, because it would result in no real change (Item removed, same Item added; Item added, same item removed)
                _collectionChangedItems.Remove(changedItem);
                return;
            }

            _collectionChangedItems.Add(collectionChangedItem);
        }

        /// <summary>
        /// Gets a value indicating whether the property was changed.
        /// </summary>
        /// <param name="expression">The property expression.</param>
        /// <returns>Returns true if the value was changed.</returns>
        public bool HasChanged<T>(Expression<Func<T, object>> expression) where T : Entity
        {
            if (_entity.IsNotSaved)
                return true;

            return HasChanged(GetPropertyName(expression));
        }

        /// <summary>
        /// Gets a value indicating whether the property was changed.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <returns>Returns true if the value was changed.</returns>
        public bool HasChanged(string propertyName)
        {
            if (_entity.IsNotSaved)
                return true;

            var entityMetadata = EntityMetadataResolver.GetEntityMetadata(_entity);
            if(entityMetadata.ListFields.Any(x => x.Name == propertyName))
            {
                return _collectionChangedItems.Any(x => x.PropertyName == propertyName);
            }

            var propertyChanges = _propertyChangedItems.Where(x => x.PropertyName == propertyName);
            if (!propertyChanges.Any())
                return false;

            var originalValue = propertyChanges.First().OldValue;
            var newValue = propertyChanges.Last().NewValue;

            if (originalValue == null && newValue == null)
                return false;

            if (originalValue == null)
                return true;

            var field = entityMetadata.Fields.FirstOrDefault(x => x.Name == propertyName);
            if(field?.IsForeignKey ?? false)
            {
                //The new value is a new instance and thats why the id is 0
                if (Convert.ToInt32(newValue) == 0)
                    return true;
            }

            if (!originalValue.Equals(newValue))
                return true;

            return false;
        }

        /// <summary>
        /// Tries the get the replaced value.
        /// </summary>
        /// <returns><c>true</c>, if the value was recovered, <c>false</c> otherwise.</returns>
        /// <param name="expression">Expression.</param>
        /// <param name="property">Property.</param>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TProperty">The property type.</typeparam>
        public bool TryGetReplacedValue<T, TProperty>(Expression<Func<T, TProperty>> expression, out TProperty property) where T : Entity
        {
            property = default(TProperty);

            var propertyChanges = _propertyChangedItems.Where(x => x.PropertyName == GetPropertyName(expression)).ToList();
            if (!propertyChanges.Any())
                return false;

            property = (TProperty)propertyChanges.First().OldValue;
            return true;
        }

        /// <summary>
        /// Tries to get the added collection items.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TEntityItem">The collection item type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="items">The added items.</param>
        /// <returns>Returns true if the changed values could be recovered.</returns>
        public bool TryGetAddedCollectionItems<T, TEntityItem>(Expression<Func<T, object>> expression, out List<TEntityItem> items) where T : Entity
        {
            return TryGetChangedCollectionItems(expression, CollectionChangeType.Added, out items);
        }

        /// <summary>
        /// Tries to get the removed collection items.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TEntityItem">The collection item type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="items">The removed items.</param>
        /// <returns>Returns true if the changed values could be recovered.</returns>
        public bool TryGetRemovedCollectionItems<T, TEntityItem>(Expression<Func<T, object>> expression, out List<TEntityItem> items) where T : Entity
        {
            return TryGetChangedCollectionItems(expression, CollectionChangeType.Removed, out items);
        }

        /// <summary>
        /// Gets the changed properties.
        /// </summary>
        /// <returns>Returns a list of changed properties.</returns>
        public List<string> GetChangedProperties()
        {
            var metadata = EntityMetadataResolver.GetEntityMetadata(_entity);
            var fields = metadata.Fields.OfType<FieldBaseMetadata>().ToList();

            return fields.Where(x => HasChanged(x.Name)).Select(property => property.Name).ToList();
        }

        /// <summary>
        /// Tries to get the changed collection items.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <typeparam name="TEntityItem">The collection item type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <param name="changeType">The collection change type.</param>
        /// <param name="items">The changed items.</param>
        /// <returns>Returns true if the changed values could be recovered.</returns>
        private bool TryGetChangedCollectionItems<T, TEntityItem>(Expression<Func<T, object>> expression, CollectionChangeType changeType, out List<TEntityItem> items) where T : Entity
        {
            items = new List<TEntityItem>();

            var collectionChanges = _collectionChangedItems.Where(x => x.PropertyName == GetPropertyName(expression) && x.CollectionChangeType == changeType);
            if (!collectionChanges.Any())
                return false;

            items = collectionChanges.Select(x => x.Item).OfType<TEntityItem>().ToList();
            return true;
        }

        /// <summary>
        /// Gets the property name out of an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the property name.</returns>
        private string GetPropertyName(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Convert:
                    return GetPropertyName(((UnaryExpression)expression).Operand);
                case ExpressionType.MemberAccess:
                    return ((MemberExpression)expression).Member.Name;
                case ExpressionType.Lambda:
                    return GetPropertyName(((LambdaExpression)expression).Body);
                default:
                    throw new InvalidOperationException($"The node type {expression.NodeType} is not supported.");
            }
        }
    }
}
