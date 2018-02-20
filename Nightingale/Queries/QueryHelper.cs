using System;
using System.Linq.Expressions;
using Nightingale.Extensions;
using Nightingale.Metadata;

namespace Nightingale.Queries
{
    internal static class QueryHelper
    {
        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the property path.</returns>
        public static string GetPropertyPath(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            MemberExpression memberExpression;

            if(lambdaExpression.Body is BinaryExpression)
            {
                var binaryExpression = lambdaExpression.Body as BinaryExpression;
                memberExpression = binaryExpression.Left as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            var propertyPath = (memberExpression.Expression + $".{memberExpression.Member.Name}");
            var lambdaParameterName = $"{lambdaExpression.Parameters[0].Name}.";
            if (propertyPath.StartsWith(lambdaParameterName))
            {
                propertyPath = propertyPath.Substring(lambdaParameterName.Length, propertyPath.Length - lambdaParameterName.Length);
            }

            return propertyPath;
        }

        /// <summary>
        /// Gets the operator symbol.
        /// </summary>
        /// <param name="op">The operator.</param>
        /// <param name="isNullType">A value indicating the type is nullable.</param>
        /// <returns>Returns the operator sql symbol.</returns>
        public static string GetOperatorSymbol(Operator op, bool isNullType = false)
        {
            if (isNullType && op == Operator.Equal)
                return "IS";

            if (isNullType && op == Operator.NotEqual)
                return "IS NOT";

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
                case Operator.Like:
                    return "LIKE";
                default:
                    throw new NotSupportedException($"The operator '{op}' was not supported.");
            }
        }

        public static Operator ConvertOperator(ExpressionType expressionType)
        {
            switch(expressionType)
            {
                case ExpressionType.Equal:
                    return Operator.Equal;
                case ExpressionType.NotEqual:
                    return Operator.NotEqual;
                case ExpressionType.GreaterThan:
                    return Operator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Operator.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return Operator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Operator.LessThanOrEqual;
                case ExpressionType.AndAlso:
                    return Operator.AndAlso;
                case ExpressionType.OrElse:
                    return Operator.OrElse;
                default:
                    throw new NotSupportedException($"The expression-type '{expressionType} was not supported.'");
            }
        }

        /// <summary>
        /// Gets a query parameter based on the specified name, value and field metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns a QueryParameter instance.</returns>
        public static QueryParameter GetQueryParameter(string name, object value, FieldMetadata fieldMetadata)
        {
            var parameter = new QueryParameter(name, value, fieldMetadata.GetSqlDbType(), !fieldMetadata.Mandatory, fieldMetadata.MaxLength);
            if (fieldMetadata.FieldType == "decimal")
            {
                parameter.Precision = fieldMetadata.DecimalPrecision;
                parameter.Scale = fieldMetadata.DecimalScale;
            }

            return parameter;
        }
    }
}
