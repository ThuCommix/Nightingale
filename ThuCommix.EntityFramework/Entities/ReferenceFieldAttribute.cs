using System;
namespace ThuCommix.EntityFramework.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ReferenceFieldAttribute : Attribute
	{
		public string ReferenceField { get; }

		public ReferenceFieldAttribute(string referenceField)
		{
			ReferenceField = referenceField;
		}
	}
}
