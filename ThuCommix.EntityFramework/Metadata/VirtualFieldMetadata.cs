using System.Xml.Serialization;

namespace ThuCommix.EntityFramework.Metadata
{
	public class VirtualFieldMetadata : FieldBaseMetadata
	{
		/// <summary>
		/// Gets or sets the expression.
		/// </summary>
		[XmlAttribute]
		public string Expression { get; set; }
	}
}
