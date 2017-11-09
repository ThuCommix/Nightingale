namespace Nightingale
{
    public class PropertyChangedItem
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Gets the old value.
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// Gets the new value.
        /// </summary>
        public object NewValue { get; }

        /// <summary>
        /// Initializes a new PropertyChangedItem class.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        public PropertyChangedItem(string propertyName, object oldValue, object newValue)
        {
            PropertyName = propertyName;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
