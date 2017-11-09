using System;

namespace Nightingale.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForeignKeyAttribute : Attribute
    {
		public string Name { get; }

		public ForeignKeyAttribute(string name)
		{
			Name = name; 
		}
	}
}
