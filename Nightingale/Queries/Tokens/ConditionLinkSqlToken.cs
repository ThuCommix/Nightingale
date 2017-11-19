using System;

namespace Nightingale.Queries.Tokens
{
    internal class ConditionLinkSqlToken : SqlToken
    {
        /// <summary>
        /// Gets the operator.
        /// </summary>
        public Operator Operator { get; }

        /// <summary>
        /// Gets the condition link type.
        /// </summary>
        public ConditionLinkType LinkType { get; }

        /// <summary>
        /// Initializes a new ConditionLinkSqlToken class.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="linkType">The condition link type.</param>
        public ConditionLinkSqlToken(Operator op, ConditionLinkType linkType) : base(SqlTokenType.ConditionLink)
        {
            Operator = op;
            LinkType = linkType;
            Sql = $" {GetOperatorSymbol(op)} ";
        }

        //TODO code duplication in ConditionSqlToken.cs

        /// <summary>
        /// Gets the operator symbol.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <returns>Returns the operator sql symbol.</returns>
        private string GetOperatorSymbol(Operator op)
        {
            switch (op)
            {
                case Operator.Equal:
                    return "=";
                case Operator.NotEqual:
                    return "!=";
                case Operator.GreaterThan:
                    return ">";
                case Operator.GreaterThanOrEqual:
                    return ">=";
                case Operator.LessThan:
                    return "<";
                case Operator.LessThanOrEqual:
                    return "<=";
                case Operator.AndAlso:
                    return "AND";
                case Operator.OrElse:
                    return "OR";
                default:
                    throw new NotSupportedException($"The expression type '{op}' was not supported.");
            }
        }
    }
}
