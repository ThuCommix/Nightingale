using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nightingale.Entities;

namespace Nightingale.Queries
{
    public class QueryConditionGroup
    {
        /// <summary>
        /// Gets the conditions.
        /// </summary>
        public IEnumerable<QueryCondition> Conditions => _conditions;

        /// <summary>
        /// Gets the junction.
        /// </summary>
        public QueryJunction Junction { get; }

        /// <summary>
        /// A value indicating whether this instance is dirty.
        /// </summary>
        internal bool IsDirty { get; set; }

        private readonly List<QueryCondition> _conditions; 

        /// <summary>
        /// Initializes a new QueryConditionGroup class.
        /// </summary>
        /// <param name="junction">The junction.</param>
        public QueryConditionGroup(QueryJunction junction = QueryJunction.And)
        {
            _conditions = new List<QueryCondition>();
            Junction = junction;
        }

        /// <summary>
        /// Creates new query condition.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the query condition.</returns>
        public QueryCondition CreateQueryCondition<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            var queryCondition = new QueryCondition(expression);
            _conditions.Add(queryCondition);

            IsDirty = true;

            return queryCondition;
        }

        /// <summary>
        /// Creates new query condition.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the query condition.</returns>
        public QueryCondition CreateQueryCondition(Expression<Func<Entity, bool>> expression)
        {
            var queryCondition = new QueryCondition(expression);
            _conditions.Add(queryCondition);

            IsDirty = true;

            return queryCondition;
        }

        /// <summary>
        /// Creates new query condition.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the query condition.</returns>
        public QueryCondition CreateQueryCondition(Expression expression)
        {
            var queryCondition = new QueryCondition(expression);
            _conditions.Add(queryCondition);

            IsDirty = true;

            return queryCondition;
        }

        /// <summary>
        /// Creates new query condition.
        /// </summary>
        /// <param name="propertyPath">The property path.</param>
        /// <param name="equationValue">The equation value.</param>
        /// <returns>Returns the query condition.</returns>
        /// <param name="expressionType">The expression type.</param>
        public QueryCondition CreateQueryCondition(string propertyPath, object equationValue, ExpressionType expressionType)
        {
            var queryCondition = new QueryCondition(propertyPath, equationValue, expressionType);
            _conditions.Add(queryCondition);

            IsDirty = true;

            return queryCondition;
        }
    }
}
