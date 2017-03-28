using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Extensions;

namespace ThuCommix.EntityFramework
{
    public class EntityCollection<T> : IEntityCollection, IList<T> where T : Entity
    {
        /// <summary>
        /// Gets or sets a collection item specified by the index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Returns a collection item.</returns>
        public T this[int index] { get => _collectionItems[index]; set => _collectionItems[index] = value; }

        /// <summary>
        /// Gets the amount of items in the collection.
        /// </summary>
        public int Count => _collectionItems.Count;

        /// <summary>
        /// A value indicating whether this collection is readonly.
        /// </summary>
        public bool IsReadOnly => false;

        private readonly string _referenceField;
        private readonly Entity _owner;
        private readonly List<T> _collectionItems;

        /// <summary>
        /// Initializes a new EntityCollection class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="referenceField">The reference field.</param>
        public EntityCollection(Entity owner, string referenceField)
        {
            _owner = owner;
            _referenceField = referenceField;
            _collectionItems = new List<T>();
        }

        /// <summary>
        /// Sets the reference field to the owner of this collection.
        /// </summary>
        /// <param name="item">The item.</param>
        private void SetReferenceField(T item)
        {
            ReflectionHelper.GetProperty(item.GetType(), _referenceField).SetValue(item, _owner);
        }

        /// <summary>
        /// Removes the reference field.
        /// </summary>
        /// <param name="item">The item.</param>
        private void RemoveReferenceField(T item)
        {
            ReflectionHelper.GetProperty(item.GetType(), _referenceField).SetValue(item, null);
        }

        /// <summary>
        /// Adds the specified elements at the end of the collection.
        /// </summary>
        /// <param name="items">The items.</param>
        public void AddRange(IEnumerable<T> items)
        {
            _collectionItems.AddRange(items);
            items.ForEach(x => SetReferenceField(x));
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
        /// Adds a new item to the collection.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(T item)
        {
            _collectionItems.Add(item);
            SetReferenceField(item);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            _collectionItems.ForEach(x => RemoveReferenceField(x));
            _collectionItems.Clear();
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
            _collectionItems.CopyTo(array, arrayIndex);
            array.ForEach(x => SetReferenceField(x));
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
            _collectionItems.Insert(index, item);
            SetReferenceField(item);
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>Returns true if the item was removed.</returns>
        public bool Remove(T item)
        {
            if(_collectionItems.Remove(item))
            {
                RemoveReferenceField(item);
                return true;
            }

            return false;
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
