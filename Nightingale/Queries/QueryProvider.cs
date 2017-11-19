using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Nightingale.Sessions;

namespace Nightingale.Queries
{
    internal class QueryProvider : IQueryProvider
    {
        /// <summary>
        /// Gets the query provider settings.
        /// </summary>
        public QueryProviderSettings Settings { get; }

        private readonly ISession _session;

        /// <summary>
        /// Initializes a new QueryProvider class.
        /// <param name="session">The session.</param>
        /// </summary>
        public QueryProvider(ISession session)
        {
            Settings = new QueryProviderSettings();
            _session = session;
        }

        /// <summary>
        /// Creates the queryable.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the IQueryable instance.</returns>
        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Queryable<>).MakeGenericType(elementType), this, expression);
            }
            catch (System.Reflection.TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        /// <summary>
        /// Creates the queryable.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the IQueryable instance.</returns>
        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return new Queryable<T>(this, expression);
        }

        /// <summary>
        /// Executes the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the result object.</returns>
        public object Execute(Expression expression)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        /// <returns>Returns the result object.</returns>
        public T Execute<T>(Expression expression)
        {
            var genericType = typeof(T);
            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(genericType);
            var entityType = isEnumerable ? genericType.GenericTypeArguments[0] : genericType;
            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression != null && methodCallExpression.Method.Name == "Count")
            {
                entityType = methodCallExpression.Arguments[0].Type.GenericTypeArguments[0];
            }

            var queryExpressionVisitor = new QueryExpressionVisitor(entityType);
            queryExpressionVisitor.RootTableAlias = Settings.RootTableAlias;
            queryExpressionVisitor.ParameterAlias = Settings.ParameterAlias;
            queryExpressionVisitor.ParameterAliasIndex = Settings.ParameterAliasStartIndex;
            queryExpressionVisitor.JoinTableAlias = Settings.JoinTableAlias;
            queryExpressionVisitor.JoinTableAliasIndex = Settings.JoinTableAliasStartIndex;

            string command;
            IQuery query;

            if (methodCallExpression != null && methodCallExpression.Method.Name == "Count")
            {
                command = queryExpressionVisitor.ParseCountQueryExpression(expression);
                query = CreateQuery(command, entityType, queryExpressionVisitor);

                return _session.ExecuteScalar<T>(query);
            }

            command = queryExpressionVisitor.ParseQueryExpression(expression);
            query = CreateQuery(command, entityType, queryExpressionVisitor);

            var entities = _session.ExecuteQuery(query);

            foreach (var include in queryExpressionVisitor.Includes)
            {
                var propertyInfo = entityType.GetProperty(include);
                foreach (var entity in entities)
                {
                    propertyInfo.GetValue(entity);
                }
            }

            if (isEnumerable)
            {
                var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(entityType));
                foreach (var entity in entities)
                {
                    resultList.Add(entity);
                }

                return (T)resultList;
            }

            if (methodCallExpression != null)
            {
                if(methodCallExpression.Method.Name == "First" && entities.Count == 0)
                    throw new InvalidOperationException("The source sequence is empty");

                if(methodCallExpression.Method.Name == "Single" && (entities.Count == 0 || entities.Count > 1))
                    throw new InvalidOperationException(entities.Count == 0 ? "The source sequence is empty" : "The source sequence contains more than one item");

                if(methodCallExpression.Method.Name == "SingleOrDefault" && entities.Count > 1)
                    throw new InvalidOperationException("The source sequence contains more than one item");
            }

            return entities.OfType<T>().FirstOrDefault();
        }

        private IQuery CreateQuery(string command, Type entityType, QueryExpressionVisitor queryExpressionVisitor)
        {
            return new Query(command, queryExpressionVisitor.ChangedEntityType ?? entityType,
                queryExpressionVisitor.Parameters);
        }
    }
}
