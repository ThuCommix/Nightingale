using System.Linq.Expressions;

namespace ThuCommix.EntityFramework.Queries
{
    public static class QueryHelper
    {
        /// <summary>
        /// Gets the property path.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the property path.</returns>
        public static string GetPropertyPath(Expression expression)
        {
            var lambdaExpression = expression as LambdaExpression;
            MemberExpression memberExpression = null;

            if(lambdaExpression.Body is BinaryExpression)
            {
                var binaryExpression = lambdaExpression.Body as BinaryExpression;
                memberExpression = binaryExpression.Left as MemberExpression;
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            var propertyPath = (memberExpression.Expression.ToString() + $".{memberExpression.Member.Name}");
            var lambdaParameterName = $"{lambdaExpression.Parameters[0].Name}.";
            if (propertyPath.StartsWith(lambdaParameterName))
            {
                propertyPath = propertyPath.Substring(lambdaParameterName.Length, propertyPath.Length - lambdaParameterName.Length);
            }

            return propertyPath;
        }
    }
}
