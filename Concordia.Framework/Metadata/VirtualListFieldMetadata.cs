﻿using System.Xml.Serialization;

namespace Concordia.Framework.Metadata
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