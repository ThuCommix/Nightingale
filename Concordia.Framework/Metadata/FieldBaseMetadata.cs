using System.Collections.Generic;
using System.Xml.Serialization;

namespace Concordia.Framework.Metadata
{
	public class FieldBaseMetadata
	{
	    [XmlIgnore]
        private static readonly List<string> SimpleTypes = new List<string> {"string", "int", "decimal", "DateTime", "bool"};

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		[XmlAttribute]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the field type.
		/// </summary>
		[XmlAttribute]
		public string FieldType { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		[XmlAttribute]
		public string Description { get; set; }

        /// <summary>
        /// A value indicating whether the field type is a complex field type.
        /// </summary>
	    [XmlIgnore]
	    public bool IsComplexFieldType => !SimpleTypes.Contains(FieldType);
	}
}
