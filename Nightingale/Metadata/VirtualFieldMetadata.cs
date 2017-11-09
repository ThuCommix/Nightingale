using System.Xml.Serialization;

namespace Nightingale.Metadata
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
