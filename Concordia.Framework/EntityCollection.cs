using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Concordia.Framework.Entities;
using Concordia.Framework.Extensions;
using Concordia.Framework.Metadata;

namespace Concordia.Framework
{
    public class EntityCollection<T> : IEntityCollection, IList<T> where T : Entity
    {
        /// <summary>
        /// Gets or sets a collection item specified by the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Returns a collection item.</returns>
        public T this[int index] { get { return _collectionItems[index]; } set { _collectionItems[index] = value; } }

        /// <summary>
        /// Gets the amount of items in the collection.
        /// </summary>
        public int Count => _collectionItems.Count;

        /// <summary>
        /// A value indicating whether this collection is readonly.
        /// </summary>
        public bool IsReadOnly => false;

        private readonly string _referenceField;
        private readonly string _propertyName;
        private readonly Entity _owner;
        private readonly List<T> _collectionItems;
        private readonly List<T> _removedCollectionItems;
        private readonly object _locker = new object();

        /// <summary>
        /// Initializes a new EntityCollection class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="referenceField">The reference field.</param>
        /// <param name="propertyName">The propertyName.</param>
        public EntityCollection(Entity owner, string referenceField, string propertyName)
        {
            _owner = owner;
            _propertyName = propertyName;
            _referenceField = referenceField;
            _collectionItems = new List<T>();
            _removedCollectionItems = new List<T>();
        }

        /// <summary>
        /// Sets the reference field to the owner of this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SetReferenceField(T item)
        {
            if (_removedCollectionItems.Contains(item))
                _removedCollectionItems.Remove(item);

            _owner.PropertyChangeTracker.AddCollectionChangedItem(_propertyName, item, CollectionChangeType.Added);
            ReflectionHelper.GetProperty(item.GetType(), _referenceField).SetValue(item, _owner);
        }

        /// <summary>
        /// Removes the reference field.
        /// </summary>
        /// <param name="item">The item.</param>
        private void RemoveReferenceField(T item)
        {
            _removedCollectionItems.Add(item);

            _owner.PropertyChangeTracker.AddCollectionChangedItem(_propertyName, item, CollectionChangeType.Removed);

            var fieldMetadata = DependencyResolver.GetInstance<IEntityMetadataResolver>().GetEntityMetadata(item).Fields.FirstOrDefault(x => x.Name == $"FK_{_referenceField}_ID");
            if (fieldMetadata?.Mandatory == true)
                return;

            ReflectionHelper.GetProperty(item.GetType(), _referenceField).SetValue(item, null);
        }

        /// <summary>
        /// Adds the specified elements at the end of the collection.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            _collectionItems.AddRange(items);
            items.ForEach(SetReferenceField);
        }

        /// <summary>
        /// Gets the collection items.
        /// </summary>
        /// <returns>Returns the collection items.</returns>
        public List<Entity> GetCollectionItems()
        {
            return _collectionItems.OfType<Entity>().ToList();
        }

        /// <summary>
        /// Gets the removed collection items.
        /// </summary>
        /// <returns>Returns the removed collection items.</returns>
        public List<Entity> GetRemovedCollectionItems()
        {
            return _removedCollectionItems.OfType<Entity>().ToList();
        }

        /// <summary>
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            lock (_locker)
            {
                _collectionItems.Add(item);
                SetReferenceField(item);
            }
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            lock (_locker)
            {
                _collectionItems.ForEach(RemoveReferenceField);
                _collectionItems.Clear();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is in the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item is in the collection.</returns>
        public bool Contains(T item)
        {
            return _collectionItems.Contains(item);
        }

        /// <summary>
        /// Copies an array of items at the specified array index.
        /// </summary>
        /// <param name="array">The source array.</param>
        /// <param name="arrayIndex">The array index.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_locker)
            {
                _collectionItems.CopyTo(array, arrayIndex);
                array.ForEach(SetReferenceField);
            }
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the IEnumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _collectionItems.GetEnumerator();
        }

        /// <summary>
        /// Gets the index of the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns the index of the specified item.</returns>
        public int IndexOf(T item)
        {
            return _collectionItems.IndexOf(item);
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="item">The item.</param>
        public void Insert(int index, T item)
        {
            lock (_locker)
            {
                _collectionItems.Insert(index, item);
                SetReferenceField(item);
            }
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item was removed.</returns>
        public bool Remove(T item)
        {
            lock (_locker)
            {
                if (_collectionItems.Remove(item))
                {
                    RemoveReferenceField(item);
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Removes an item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            Remove(_collectionItems[index]);
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the IEnumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collectionItems.GetEnumerator();
        }
    }
}
