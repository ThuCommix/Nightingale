using System.Xml.Serialization;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Metadata
{
	public class ListFieldMetadata : FieldBaseMetadata
	{
		/// <summary>
		/// Gets or sets the cascade.
		/// </summary>
		[XmlAttribute]
		public Cascade Cascade { get; set; }

		/// <summary>
		/// Gets the reference field in case this field is a list.
		/// </summary>
		[XmlAttribute]
		public string ReferenceField { get; set; }

        /// <summary>
        /// A value indicating whether the list should be loaded in eager-mode.
        /// </summary>
        [XmlAttribute]
        public bool EagerLoad { get; set; }
	}
}
