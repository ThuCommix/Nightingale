using System;
using System.Linq.Expressions;

namespace Nightingale.Queries
{
    public class SortExpression
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the sorting mode.
        /// </summary>
        public SortingMode Sorting { get; }

        /// <summary>
        /// Initializes a new SortExpression class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="sorting">The sorting mode.</param>
        public SortExpression(Expression expression, SortingMode sorting)
        {
            if (expression == null)
                throw new ArgumentNullException(nameof(expression));

            Expression = expression;
            Sorting = sorting;
        }
    }
}
