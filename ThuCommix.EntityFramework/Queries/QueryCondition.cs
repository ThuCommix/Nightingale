using System;
using System.Linq.Expressions;

namespace ThuCommix.EntityFramework.Queries
{
    public class QueryCondition
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the property path.
        /// </summary>
        public string PropertyPath { get; }

        /// <summary>
        /// Gets the equation value.
        /// </summary>
        public object EquationValue { get; }

        /// <summary>
        /// Initializes a new QueryCondition class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public QueryCondition(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            PropertyPath = GetPropertyPath(expression);
            EquationValue = GetEquationValue(expression);
        }

        /// <summary>
        /// Initializes a new QueryCondition class.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="equationValue">The equation value.</param>
        public QueryCondition(string propertyPath, object equationValue)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
                throw new ArgumentNullException(nameof(propertyPath));

            PropertyPath = propertyPath;
            EquationValue = equationValue;
        }

        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the property path.</returns>
        private string GetPropertyPath(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var binaryExpression = lambdaExpression.Body as BinaryExpression;
            var memberExpression = binaryExpression.Left as MemberExpression;

            var propertyPath = (memberExpression.Expression.ToString() + $".{memberExpression.Member.Name}");
            var lambdaParameterName = $"{lambdaExpression.Parameters[0].Name}.";
            if (propertyPath.StartsWith(lambdaParameterName))
            {
                propertyPath = propertyPath.Substring(lambdaParameterName.Length, propertyPath.Length - lambdaParameterName.Length);
            }

            return propertyPath;
        }

        /// <summary>
        /// Gets the equation value.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the equation value.</returns>
        private object GetEquationValue(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var binaryExpression = lambdaExpression.Body as BinaryExpression;

            return Expression.Lambda(binaryExpression.Right).Compile().DynamicInvoke();
        }
    }
}
