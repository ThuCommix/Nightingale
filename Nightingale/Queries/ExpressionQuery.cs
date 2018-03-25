using System.Linq.Expressions;

namespace Nightingale.Queries
{
    internal class ExpressionQuery : ExpressionVisitor
    {
        /// <summary>
        /// Gets the new expression.
        /// </summary>
        public NewExpression NewExpression { get; private set; }

        /// <summary>
        /// Gets the member init expression.
        /// </summary>
        public MemberInitExpression MemberInitExpression { get; private set; }

        /// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.NewExpression"></see>.</summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitNew(NewExpression node)
        {
            NewExpression = node;
            return base.VisitNew(node);
        }

        /// <summary>Visits the children of the <see cref="T:System.Linq.Expressions.MemberInitExpression"></see>.</summary>
        /// <param name="node">The expression to visit.</param>
        /// <returns>The modified expression, if it or any subexpression was modified; otherwise, returns the original expression.</returns>
        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            MemberInitExpression = node;
            return base.VisitMemberInit(node);
        }
    }
}
