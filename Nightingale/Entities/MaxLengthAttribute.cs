using System;

namespace Nightingale.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MaxLengthAttribute : Attribute
    {
        /// <summary>
        /// Gets the max length.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        /// Initializes a new MaxLengthAttribute class.
        /// </summary>
        /// <param name="maxLength">The max length.</param>
        public MaxLengthAttribute(int maxLength)
        {
            MaxLength = maxLength;
        }
    }
}
