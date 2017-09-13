using System.Collections.Generic;
using System.Xml.Serialization;

namespace Concordia.Framework.Metadata
{
    [XmlRoot("Entity")]
    public class EntityMetadata
    {
		private string _table;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table name.
        /// </summary>
        [XmlAttribute]
        public string Table 
		{
			get => string.IsNullOrWhiteSpace(_table) ? Name : _table;
            set => _table = value;
        }

        /// <summary>
        /// Gets or sets the namespace.
        /// </summary>
        [XmlAttribute]
        public string Namespace { get; set; }

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		[XmlAttribute]
		public string Description { get; set; }

        /// <summary>
        /// Gets or sets the fields.
        /// </summary>
        [XmlArray, XmlArrayItem("Field")]
        public List<FieldMetadata> Fields { get; set; }

        /// <summary>
        /// Gets or sets the list fields.
        /// </summary>
        [XmlArray, XmlArrayItem("ListField")]
        public List<ListFieldMetadata> ListFields { get; set; }

        /// <summary>
        /// gets or sets the virtual fields.
        /// </summary>
        [XmlArray, XmlArrayItem("VirtualField")]
        public List<VirtualFieldMetadata> VirtualFields { get; set; }

        /// <summary>
        /// gets or sets the virtual list fields.
        /// </summary>
        [XmlArray, XmlArrayItem("VirtualListField")]
        public List<VirtualListFieldMetadata> VirtualListFields { get; set; } 

        /// <summary>
        /// Initializes a new EntityMetadata class.
        /// </summary>
        public EntityMetadata()
        {
            Fields = new List<FieldMetadata>();
            ListFields = new List<ListFieldMetadata>();
            VirtualFields = new List<VirtualFieldMetadata>();
            VirtualListFields = new List<VirtualListFieldMetadata>();
        } 
    }
}
