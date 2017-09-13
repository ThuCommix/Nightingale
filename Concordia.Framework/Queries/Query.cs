using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Concordia.Framework.Entities;
using Concordia.Framework.Extensions;
using Concordia.Framework.Metadata;
using Concordia.Framework.Queries.Tokens;

namespace Concordia.Framework.Queries
{
    public class Query : IQuery
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command
        {
            get
            {
                if (ConditionGroups.Any(x => x.IsDirty))
                    CompileSql();

                ConditionGroups.ForEach(x => x.IsDirty = false);

                return _command;
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<QueryParameter> Parameters => _parameters;

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Gets or sets the maximum results.
        /// </summary>
        public int? MaxResults { get; set; }

        /// <summary>
        /// Gets the condition groups.
        /// </summary>
        public IEnumerable<QueryConditionGroup> ConditionGroups => _groups;

        /// <summary>
        /// Gets the sorting expressions.
        /// </summary>
        public IEnumerable<SortExpression> SortingExpressions => _sortingExpressions;

        /// <summary>
        /// Gets the global query filters.
        /// </summary>
        public static IEnumerable<GlobalQueryFilter> GlobalQueryFilters => GlobalQueryFiltersList;

        private static readonly List<GlobalQueryFilter> GlobalQueryFiltersList = new List<GlobalQueryFilter>();

        /// <summary>
        /// Gets the entity metadata resolver.
        /// </summary>
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        /// <summary>
        /// Gets the sql token composer service.
        /// </summary>
        protected ISqlTokenComposerService SqlTokenComposerService => DependencyResolver.GetInstance<ISqlTokenComposerService>();

        private readonly List<SortExpression> _sortingExpressions;
        private readonly List<QueryParameter> _parameters;
        private readonly List<QueryConditionGroup> _groups;
        private string _command;

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        public Query()
        {
            _groups = new List<QueryConditionGroup>();
            _sortingExpressions = new List<SortExpression>();
            _parameters = new List<QueryParameter>();
        }

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="parameters">The query parameters.</param>
        public Query(string command, Type entityType, IEnumerable<QueryParameter> parameters = null) : this()
        {
            _command = command;
            EntityType = entityType;

            if(parameters != null)
                _parameters.AddRange(parameters);

            ApplyGlobalQueryFilters();
        }

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        private Query(Type entityType) : this()
        {
            EntityType = entityType;

            ApplyGlobalQueryFilters();
        }

        /// <summary>
        /// Creates a query condition group with the specified junction.
        /// </summary>
        /// <param name="junction">The junction.</param>
        /// <returns>Returns the query condition group.</returns>
        public QueryConditionGroup CreateQueryConditionGroup(QueryJunction junction = QueryJunction.And)
        {
            var queryConditionGroup = new QueryConditionGroup(junction);
            _groups.Add(queryConditionGroup);

            return queryConditionGroup;
        }

        /// <summary>
        /// Adds a new query condition group.
        /// </summary>
        /// <param name="conditionGroup">The query condition group.</param>
        public void AddQueryConditionGroup(QueryConditionGroup conditionGroup)
        {
            if (conditionGroup == null)
                throw new ArgumentNullException(nameof(conditionGroup));

            _groups.Add(conditionGroup);
        }

        /// <summary>
        /// Adds a new sorting expression to the query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="sortingExpression">The sorting expression.</param>
        /// <param name="sorting">The sorting mode.</param>
        public void AddSortingExpression<T>(Expression<Func<T, object>> sortingExpression, SortingMode sorting) where T : Entity
        {
            if (sortingExpression == null)
                throw new ArgumentNullException(nameof(sortingExpression));

            AddSortingExpression(new SortExpression(sortingExpression, sorting));
        }

        /// <summary>
        /// Adds a new sorting expression to the query.
        /// </summary>
        /// <param name="sortExpression">The sort expression.</param>
        public void AddSortingExpression(SortExpression sortExpression)
        {
            if (sortExpression == null)
                throw new ArgumentNullException(nameof(sortExpression));

            _sortingExpressions.Add(sortExpression);
        }

        /// <summary>
        /// Adds a new query parameter.
        /// </summary>
        /// <param name="parameter">The query parameter.</param>
        public void AddQueryParameter(QueryParameter parameter)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter));

            _parameters.Add(parameter);
        }

        private void CompileSql()
        {
            var entityMetadata = EntityMetadataResolver.GetEntityMetadata(EntityType);
            var rootSelectToken = new SelectSqlToken(entityMetadata);
            var sqlTokens = new List<SqlToken> { rootSelectToken };
            var aliasIndex = 0;
            var propertyAliasMapping = new Dictionary<string, string> {{string.Empty, entityMetadata.Table.ToLower()}};

            foreach (var group in ConditionGroups)
            {
                sqlTokens.Add(new ConditionLinkSqlToken(group.Junction == QueryJunction.And ? Operator.AndAlso : Operator.OrElse, ConditionLinkType.Start));

                foreach(var condition in group.Conditions)
                {
                    // The expression can be null, in this case the PropertyPath, EquationValue and Binary operator is set
                    // TypeContextToken -> PropertyToken -> BinaryToken -> ConstantToken

                    List<Token> tokens;

                    if (condition.Expression == null)
                    {
                        tokens = new List<Token> { new TypeContextToken(EntityType), new PropertyToken(condition.PropertyPath),
                            new BinaryToken(QueryHelper.ConvertOperator(condition.ExpressionType)), new ConstantToken(condition.EquationValue) };
                    }
                    else
                    {
                        var expressionTokenizer = new ExpressionTokenizer();
                        expressionTokenizer.Eval(condition.Expression);
                        tokens = expressionTokenizer.Tokens;
                    }

                    var conditionType = tokens.OfType<TypeContextToken>().First().Type;

                    if (conditionType != EntityType && conditionType != typeof(Entity))
                        throw new QueryException("The condition type must match the entity type of the query.");

                    var currentEntityMetadata = entityMetadata;
                    var currentAlias = rootSelectToken.Alias;
                    var propertyPath = string.Empty;

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        var currentToken = tokens[i];
                        if(currentToken.TokenType == TokenType.Constant)
                        {
                            if(i + 1 < tokens.Count && tokens[i + 1].TokenType == TokenType.Binary)
                            {
                                sqlTokens.Add(new ConditionLinkSqlToken(((BinaryToken)tokens[i + 1]).Operator, ConditionLinkType.Between));
                            }
                        }

                        if(currentToken.TokenType == TokenType.Property)
                        {
                            var propertyToken = (PropertyToken)currentToken;
                            var fieldMetadata = currentEntityMetadata.Fields.FirstOrDefault(x => x.Name == propertyToken.Property || x.Name == $"FK_{propertyToken.Property}_ID");

                            if(tokens[i + 1].TokenType == TokenType.Binary)
                            {
                                var binaryToken = (BinaryToken)tokens[i + 1];
                                var constantToken = (ConstantToken)tokens[i + 2]; // could be a problem for x => x.IsOk && .. instead of x => x.IsOk == true &&
                                // this is a single property access without a join.
                                sqlTokens.Add(new ConditionSqlToken(binaryToken.Operator, currentAlias, fieldMetadata, constantToken.Value));
                                // Reset current alias
                                currentAlias = rootSelectToken.Alias;
                                propertyPath = string.Empty;
                            }
                            else if(tokens[i + 1].TokenType == TokenType.Method)
                            {
                                // method call on the property
                                var methodToken = tokens[i + 1] as MethodToken;
                                var constantToken = (ConstantToken)tokens[i + 2]; // could be a problem for x => x.IsOk && .. instead of x => x.IsOk == true &&

                                if (methodToken.MethodName == "StartsWith" && methodToken.DeclaringType == typeof(string))
                                {
                                    sqlTokens.Add(new ConditionSqlToken(Operator.Like, currentAlias, fieldMetadata, $"{constantToken.Value}%"));
                                }
                                else if (methodToken.MethodName == "EndsWith" && methodToken.DeclaringType == typeof(string))
                                {
                                    sqlTokens.Add(new ConditionSqlToken(Operator.Like, currentAlias, fieldMetadata, $"%{constantToken.Value}"));
                                }
                                else if (methodToken.MethodName == "Contains" && methodToken.DeclaringType == typeof(string))
                                {
                                    sqlTokens.Add(new ConditionSqlToken(Operator.Like, currentAlias, fieldMetadata, $"%{constantToken.Value}%"));
                                }
                                else
                                {
                                    throw new NotSupportedException($"The method {methodToken.MethodName} was not supported.");
                                }

                                // Reset current alias
                                currentAlias = rootSelectToken.Alias;
                                propertyPath = string.Empty;
                            }
                            else if (tokens[i + 1].TokenType == TokenType.Property)
                            {
                                // This is just a join to reach another property
                                var targetEntityMetadata = EntityMetadataResolver.EntityMetadata.FirstOrDefault(x => x.Name == fieldMetadata.FieldType);
                                var existingJoinSqlToken = sqlTokens.OfType<JoinSqlToken>().FirstOrDefault(x => x.SourceAlias == currentAlias && x.NavigationFieldMetadata == fieldMetadata);
                                if (existingJoinSqlToken == null)
                                {
                                    var joinSqlToken = new JoinSqlToken(aliasIndex++, currentAlias, currentEntityMetadata, fieldMetadata, targetEntityMetadata);
                                    sqlTokens.Add(joinSqlToken);
                                    currentAlias = joinSqlToken.TargetAlias;
                                }
                                else
                                {
                                    currentAlias = existingJoinSqlToken.TargetAlias;
                                }

                                propertyPath += string.IsNullOrWhiteSpace(propertyPath) ? propertyToken.Property : $".{propertyToken.Property}";
                                if(!propertyAliasMapping.ContainsKey(propertyPath))
                                    propertyAliasMapping.Add(propertyPath, currentAlias);

                                currentEntityMetadata = targetEntityMetadata;
                            }
                            else
                            {
                                // Should not happen ;)
                                throw new QueryException("Invalid token syntax.");
                            }
                        }
                    }

                    if(condition != group.Conditions.Last())
                        sqlTokens.Add(new ConditionLinkSqlToken(Operator.AndAlso, ConditionLinkType.Between));
                }

                sqlTokens.Add(new ConditionLinkSqlToken(Operator.Equal, ConditionLinkType.End));
            }

            var result = SqlTokenComposerService.ComposeSql(sqlTokens);
            _command = result.Command;
            _parameters.Clear();
            _parameters.AddRange(result.Parameters);

            if(_sortingExpressions.Count > 0)
                _command += $" ORDER BY {ResolveSortingExpressions(_sortingExpressions, propertyAliasMapping)}";

            if (MaxResults != null)
                _command += $" LIMIT {MaxResults.Value}";
        }

        private void ApplyGlobalQueryFilters()
        {
            var filters = GlobalQueryFilters.Where(x => x.EntityType == EntityType).ToList();
            if(filters.Count > 0)
            {
                var group = new QueryConditionGroup();
                filters.ForEach(x => group.CreateQueryCondition(x.Expression));
                _groups.Insert(0, group);
            }
        }

        private static string ResolveSortingExpressions(IEnumerable<SortExpression> sortingExpressions, Dictionary<string, string> joins)
        {
            var sortings = new List<string>();

            foreach(var sortExpression in sortingExpressions)
            {
                var propertyPath = QueryHelper.GetPropertyPath(sortExpression.Expression);
                var lastIndex = propertyPath.LastIndexOf('.');
                var basePropertyPath = lastIndex > 0 ? propertyPath.Substring(0, propertyPath.LastIndexOf('.')) : string.Empty;
                var aliasWithPath = joins.FirstOrDefault(x => x.Key == basePropertyPath);
                if (aliasWithPath.Key == null)
                    throw new QueryException("The sort expression can not be resolved because the selected entity is not joined.");

                sortings.Add($"{aliasWithPath.Value}.{propertyPath.Split('.').Last()} {GetSortingName(sortExpression.Sorting)}");
            }

            return string.Join(", ", sortings);
        }

        private static string GetSortingName(SortingMode sorting)
        {
            return sorting == SortingMode.Ascending ? "ASC" : "DESC";
        }

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns a query.</returns>
        public static Query CreateQuery<T>() where T : Entity
        {
            return new Query(typeof(T));
        }

        /// <summary>
        /// Gets a query parameter based on the specified name, value and field metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns a QueryParameter instance.</returns>
        public static QueryParameter GetQueryParameter(string name, object value, FieldMetadata fieldMetadata)
        {
            return new QueryParameter(name, value, fieldMetadata.GetSqlDbType(), !fieldMetadata.Mandatory, fieldMetadata.MaxLength);
        }

        /// <summary>
        /// Sets a global query filter.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        public static void SetQueryFilter<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            GlobalQueryFiltersList.Add(GlobalQueryFilter.CreateQueryFilter(expression));
        }

        /// <summary>
        /// Removes all query filters for the specified entity type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        public static void RemoveQueryFilters<T>()
        {
            GlobalQueryFiltersList.RemoveAll(x => x.EntityType == typeof(T));
        }
    }
}
