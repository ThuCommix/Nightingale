using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
        private readonly IEntityService _entityService;

        /// <summary>
        /// Initializes a new QueryProvider class.
        /// <param name="session">The session.</param>
        /// <param name="entityService">The entity service.</param>
        /// </summary>
        public QueryProvider(ISession session, IEntityService entityService)
        {
            Settings = new QueryProviderSettings();
            _session = session;
            _entityService = entityService;
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
            if (methodCallExpression != null && (methodCallExpression.Method.Name == "Count" || methodCallExpression.Method.Name == "Select"))
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

            if (queryExpressionVisitor.IsCustomSelect)
            {
                return HandleCustomQuerySelect<T>(command, queryExpressionVisitor, expression);
            }

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

        private T HandleCustomQuerySelect<T>(string command, QueryExpressionVisitor visitor, Expression expression)
        {
            var resultType = typeof(T).GenericTypeArguments[0];
            var resultSet = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(resultType));
            var expressionQuery = new ExpressionQuery();
            expressionQuery.Visit(expression);

            var constructorParameters = expressionQuery.NewExpression?.Constructor.GetParameters();
            var parameterCount = constructorParameters?.Length ?? 0;

            using (var reader = _session.Connection.ExecuteReader(new Query(command, null, visitor.Parameters)))
            {
                while (reader.Read())
                {
                    var parameters = new List<object>();
                    var members = new List<object>();

                    foreach (var selectDescription in visitor.SelectDescriptions)
                    {
                        var result = selectDescription.IsFullEntity
                            ? _entityService.CreateEntity(reader, selectDescription.Metadata, $"{selectDescription.Identifier}_")
                            : reader[$"{selectDescription.Identifier}_{selectDescription.Alias}"];

                        if (result is DBNull)
                        {
                            result = null;
                        }

                        if (parameters.Count < parameterCount)
                        {
                            if (result != null)
                            {
                                var parameterType = constructorParameters[parameters.Count].ParameterType;
                                var underlyingParameterType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;

                                parameters.Add(Convert.ChangeType(result, underlyingParameterType));
                            }
                            else
                            {
                                parameters.Add(null);
                            }
                        }
                        else
                        {
                            members.Add(result);
                        }
                    }

                    object resultObject;
                    if (expressionQuery.NewExpression != null)
                    {
                        resultObject = expressionQuery.NewExpression.Constructor.Invoke(parameters.ToArray());
                    }
                    else
                    {
                        var underlyingParameterType = Nullable.GetUnderlyingType(resultType) ?? resultType;
                        resultObject = Convert.ChangeType(members[0], underlyingParameterType);
                    }

                    if (expressionQuery.MemberInitExpression != null)
                    {
                        var membersResolved = 0;
                        foreach (var memberBinding in expressionQuery.MemberInitExpression.Bindings)
                        {
                            if (members[membersResolved] == null)
                            {
                                membersResolved++;
                                continue;
                            }

                            if (memberBinding.Member is PropertyInfo propertyInfo)
                            {
                                var underlyingType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                                propertyInfo.SetValue(resultObject, Convert.ChangeType(members[membersResolved++], underlyingType));
                            }
                            else if (memberBinding.Member is FieldInfo fieldInfo)
                            {
                                var underlyingType = Nullable.GetUnderlyingType(fieldInfo.FieldType) ?? fieldInfo.FieldType;
                                fieldInfo.SetValue(resultObject, Convert.ChangeType(members[membersResolved++], underlyingType));
                            }
                            else
                            {
                                throw new NotSupportedException($"MemberBinding with type '{memberBinding.Member.GetType().Name}' is not supported.");
                            }
                        }
                    }

                    resultSet.Add(resultObject);
                }
            }

            return (T) resultSet;
        }

        private IQuery CreateQuery(string command, Type entityType, QueryExpressionVisitor queryExpressionVisitor)
        {
            return new Query(command, queryExpressionVisitor.ChangedEntityType ?? entityType,
                queryExpressionVisitor.Parameters);
        }
    }
}
