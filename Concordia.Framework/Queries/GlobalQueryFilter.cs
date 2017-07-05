using System;
using System.Linq.Expressions;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Queries
{
    public class GlobalQueryFilter
    {
        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public Type EntityType { get; }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        public Expression Expression { get; }

        /// <summary>
        /// Initializes a new GlobalQueryFilter class.
        /// </summary>
        /// <param name="type">The entity type.</param>
        /// <param name="expression">The expression.</param>
        public GlobalQueryFilter(Type type, Expression expression)
        {
            EntityType = type;
            Expression = expression;
        }

        /// <summary>
        /// Creates a new query filter.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns a GlobalQueryFilter instance.</returns>
        public static GlobalQueryFilter CreateQueryFilter<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            return new GlobalQueryFilter(typeof(T), expression);
        }
    }
}
