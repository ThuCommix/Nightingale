using System;
using System.Xml.Serialization;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Metadata
{
    [Serializable]
    public class FieldMetadata : FieldBaseMetadata
	{
		/// <summary>
		/// A value indicating whether this field should be unique.
		/// </summary>
		[XmlAttribute]
		public bool Unique { get; set; }

		/// <summary>
		/// Gets or sets the cascade.
		/// </summary>
		[XmlAttribute]
		public Cascade Cascade { get; set; }

        /// <summary>
        /// A value indicating whether the field is mandatory.
        /// </summary>
        [XmlAttribute]
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the max length for string based properties.
        /// </summary>
        [XmlAttribute]
        public int MaxLength { get; set; }

		/// <summary>
		/// Gets or sets the decimal precision.
		/// </summary>
		[XmlAttribute]
		public int DecimalPrecision { get; set; }

		/// <summary>
		/// Gets or sets the decimal scale.
		/// </summary>
		[XmlAttribute]
		public int DecimalScale { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the time should be truncated.
		/// </summary>
		[XmlAttribute]
		public bool DateOnly { get; set; }

        /// <summary>
        /// Gets a value indicating whether the field is a foreign key.
        /// </summary>
        [XmlIgnore]
        public bool IsForeignKey => !string.IsNullOrWhiteSpace(ForeignKey);

        /// <summary>
        /// Gets or sets the name of the foreign key property.
        /// </summary>
        [XmlIgnore]
        public string ForeignKey { get; set; }

        /// <summary>
        /// A value indicating whether the field should be loaded in eager-mode.
        /// </summary>
        [XmlAttribute]
        public bool EagerLoad { get; set; }

        /// <summary>
        /// A value indicating whether the field is a enum.
        /// </summary>
        [XmlAttribute]
        public bool Enum { get; set; }
    }
}
