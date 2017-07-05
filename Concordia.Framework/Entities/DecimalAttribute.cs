using System;

namespace Concordia.Framework.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class DecimalAttribute : Attribute
	{
		public int Precision { get; }

		public int Scale { get; }

		public DecimalAttribute(int precision, int scale)
		{
			Precision = precision;
			Scale = scale;
		}
	}
}
