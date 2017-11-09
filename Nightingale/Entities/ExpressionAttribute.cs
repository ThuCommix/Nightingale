using System;

namespace Nightingale.Entities
{
	[AttributeUsage(AttributeTargets.Property)]
	public class ExpressionAttribute : Attribute
	{
		public string Expression { get; }

		public ExpressionAttribute(string expression)
		{
			Expression = expression;
		}
	}
}
