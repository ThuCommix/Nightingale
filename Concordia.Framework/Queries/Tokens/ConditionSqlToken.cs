using System;
using Concordia.Framework.Metadata;

namespace Concordia.Framework.Queries.Tokens
{
    public class ConditionSqlToken : SqlToken
    {
        /// <summary>
        /// Gets the operator.
        /// </summary>
        public Operator Operator { get; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets te field metadata.
        /// </summary>
        public FieldMetadata FieldMetadata { get; }

        /// <summary>
        /// Gets the comparation value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new ConditionSqlToken class.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="fieldMetadata">The field metadata</param>
        /// <param name="value">The comparation object.</param>
        public ConditionSqlToken(Operator op, string alias, FieldMetadata fieldMetadata, object value) : base(SqlTokenType.Condition)
        {
            Operator = op;
            Alias = alias;
            FieldMetadata = fieldMetadata;
            Value = value;
            Sql = $"{alias}.{fieldMetadata.Name} {QueryHelper.GetOperatorSymbol(op, value == null)}";
        }
    }
}
