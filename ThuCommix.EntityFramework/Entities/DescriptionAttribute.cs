using System;
namespace ThuCommix.EntityFramework.Entities
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
	public class DescriptionAttribute : Attribute
	{
		/// <summary>
		/// Gets the description.
		/// </summary>
		public string Description { get; }

		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}
