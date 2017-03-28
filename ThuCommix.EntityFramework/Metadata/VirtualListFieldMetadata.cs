using System.Xml.Serialization;

namespace ThuCommix.EntityFramework.Metadata
{
	public class VirtualListFieldMetadata : FieldBaseMetadata
	{
		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		[XmlAttribute]
		public string Expression { get; set; }
	}
}
