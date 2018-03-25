using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nightingale.Sessions;

namespace Nightingale.Queries
{
    internal class Queryable<T> : IOrderedQueryable<T>
    {
        /// <summary>
        /// Gets the query provider.
        /// </summary>
        public IQueryProvider Provider { get; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Gets the element type.
        /// </summary>
        public Type ElementType => typeof(T);

        /// <summary>
        /// Initializes the Queryable class.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="entityService">The entity service.</param>
        public Queryable(ISession session, IEntityService entityService)
        {
            Provider = new QueryProvider(session, entityService);
            Expression = Expression.Constant(this);
        }

        /// <summary>
        /// Initializes the Queryable class.
        /// </summary>
        /// <param name="provider">The query provider.</param>
        /// <param name="expression">The expression.</param>
        public Queryable(IQueryProvider provider, Expression expression)
        {
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));

            if(expression == null)
                throw new ArgumentNullException(nameof(expression));

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
                throw new ArgumentOutOfRangeException("expression");

            Provider = provider;
            Expression = expression;
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerable<T>>(Expression).GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns>Returns the enumerator.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Provider.Execute<System.Collections.IEnumerable>(Expression).GetEnumerator();
        }
    }
}
