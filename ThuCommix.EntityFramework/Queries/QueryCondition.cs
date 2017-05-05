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
        /// Gets the expression type.
        /// </summary>
        public ExpressionType ExpressionType { get; }

        /// <summary>
        /// Initializes a new QueryCondition class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public QueryCondition(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            PropertyPath = QueryHelper.GetPropertyPath(expression);
            EquationValue = GetEquationValue(expression);
            ExpressionType = GetExpressionType(expression);
        }

        /// <summary>
        /// Initializes a new QueryCondition class.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="equationValue">The equation value.</param>
        /// <param name="expressionType">The expression type.</param>
        public QueryCondition(string propertyPath, object equationValue, ExpressionType expressionType)
        {
            if (string.IsNullOrWhiteSpace(propertyPath))
                throw new ArgumentNullException(nameof(propertyPath));

            PropertyPath = propertyPath;
            EquationValue = equationValue;
            ExpressionType = expressionType;
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

        /// <summary>
        /// Gets the expression type.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the expression type.</returns>
        private ExpressionType GetExpressionType(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            var binaryExpression = lambdaExpression.Body as BinaryExpression;

            return binaryExpression.NodeType;
        }
    }
}
