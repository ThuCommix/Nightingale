using System;

namespace ThuCommix.EntityFramework.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class CascadeAttribute : Attribute
	{
		public Cascade Cascade { get; }

		public CascadeAttribute(Cascade cascade)
		{
			Cascade = cascade;
		}
	}
}
