using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ThuCommix.EntityFramework.Entities;
using ThuCommix.EntityFramework.Extensions;
using ThuCommix.EntityFramework.Metadata;

namespace ThuCommix.EntityFramework.Queries
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
        }

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        private Query(Type entityType) : this()
        {
            EntityType = entityType;
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
            var entityMetadataResolver = DependencyResolver.GetInstance<IEntityMetadataResolver>();
            var metadata = entityMetadataResolver.GetEntityMetadata(EntityType);
            var commandBuilder = new StringBuilder();
            var entityName = EntityType.Name.ToLower();
            var parameters = new List<QueryParameter>();
            var joins = new List<Tuple<string, string>> { new Tuple<string, string>(string.Empty, entityName) };
            var joinCommands = new List<string>();
            var conditionGroupCommands = new List<string>();

            commandBuilder.AppendLine($"SELECT {string.Join(", ", metadata.Fields.Select(x => x.Name))} FROM {metadata.Table} {entityName}");

            var parameterIndex = 0;
            var aliasIndex = 0;

            foreach(var group in ConditionGroups)
            {
                var conditionCommands = new List<string>();

                foreach(var condition in group.Conditions)
                {
                    var propertyPath = condition.PropertyPath;
                    var equationValue = condition.EquationValue;
                    var currentMetadata = metadata;
                    var propertyList = propertyPath.Split('.');
                    var previousAlias = entityName;
                    var parameterName = GetParameterName(ref parameterIndex);
                    var currentPropertyPath = string.Empty;
                    var operatorSymbol = GetOperatorSymbol(condition);
                    FieldMetadata fieldMetadata = null;

                    foreach (var propertyName in propertyList)
                    {
                        currentPropertyPath += currentPropertyPath == string.Empty ? propertyName : $".{propertyName}";
                        fieldMetadata = currentMetadata.Fields.FirstOrDefault(x => x.Name == propertyName || x.Name == $"FK_{propertyName}_ID");
                        if (!fieldMetadata.IsComplexFieldType && !fieldMetadata.IsForeignKey)
                        {
                            conditionCommands.Add($"{previousAlias}.{fieldMetadata.Name} {operatorSymbol} {parameterName}");
                        }
                        else
                        {
                            if (propertyList.Length == 1)
                            {
                                conditionCommands.Add($"{previousAlias}.{fieldMetadata.Name} {operatorSymbol} {parameterName}");
                                break;
                            }

                            var existingAlias = joins.FirstOrDefault(x => x.Item1 == currentPropertyPath);
                            if(existingAlias != null)
                            {
                                previousAlias = existingAlias.Item2;
                            }
                            else
                            {
                                var aliasName = GetAlias(ref aliasIndex);
                                joinCommands.Add($"{GetJoinType(fieldMetadata)} JOIN {fieldMetadata.FieldType} {aliasName} ON {aliasName}.Id = {previousAlias}.{fieldMetadata.Name}");
                                previousAlias = aliasName;

                                joins.Add(new Tuple<string, string>(currentPropertyPath, aliasName));
                            }

                            currentMetadata = entityMetadataResolver.EntityMetadata.FirstOrDefault(x => x.Name == fieldMetadata.FieldType);
                        }
                    }

                    parameters.Add(GetQueryParameter(parameterName, equationValue, fieldMetadata));
                }

                conditionGroupCommands.Add(string.Join(" AND ", conditionCommands));
            }

            foreach(var joinCommand in joinCommands)
            {
                commandBuilder.AppendLine(joinCommand);
            }

            commandBuilder.AppendLine("WHERE");

            for (var i = 0; i < conditionGroupCommands.Count; i++)
            {
                commandBuilder.AppendLine($"({conditionGroupCommands[i]})");
                if(i + 1 < conditionGroupCommands.Count)
                {
                    var conditionGroup = ConditionGroups.ToList()[i + 1];
                    commandBuilder.AppendLine(conditionGroup.Junction == QueryJunction.And ? "AND" : "OR");
                }
            }

            if (_sortingExpressions.Count > 0)
            {
                commandBuilder.AppendLine($"ORDER BY {ResolveSortingExpressions(_sortingExpressions, joins)}");
            }

            if (MaxResults != null)
            {
                commandBuilder.AppendLine($"LIMIT {MaxResults}");
            }

            _command = commandBuilder.ToString().Replace(Environment.NewLine, " ");

            _parameters.Clear();
            _parameters.AddRange(parameters);
        }

        private static string ResolveSortingExpressions(IEnumerable<SortExpression> sortingExpressions, IEnumerable<Tuple<string, string>> joins)
        {
            var sortings = new List<string>();

            foreach(var sortExpression in sortingExpressions)
            {
                var propertyPath = QueryHelper.GetPropertyPath(sortExpression.Expression);
                var lastIndex = propertyPath.LastIndexOf('.');
                var basePropertyPath = lastIndex > 0 ? propertyPath.Substring(0, propertyPath.LastIndexOf('.')) : string.Empty;
                var aliasWithPath = joins.FirstOrDefault(x => x.Item1 == basePropertyPath);
                if (aliasWithPath == null)
                    throw new QueryException("The sort expression can not be resolved because the selected entity is not joined.");

                sortings.Add($"{aliasWithPath.Item2}.{propertyPath.Split('.').Last()} {GetSortingName(sortExpression.Sorting)}");
            }

            return string.Join(", ", sortings);
        }

        private static string GetSortingName(SortingMode sorting)
        {
            return sorting == SortingMode.Ascending ? "ASC" : "DESC";
        }

        private static string GetParameterName(ref int parameterIndex)
        {
            return $"@p{parameterIndex++}";
        }

        private static string GetAlias(ref int aliasIndex)
        {
            return $"a{aliasIndex++}";
        }

        private static string GetJoinType(FieldMetadata field)
        {
            return field.Mandatory ? "INNER" : "LEFT";
        }

        private string GetOperatorSymbol(QueryCondition condition)
        {
            switch(condition.ExpressionType)
            {
                case ExpressionType.Equal:
                    return condition.EquationValue == null ? "IS" : "=";
                case ExpressionType.NotEqual:
                    return condition.EquationValue == null ? "IS NOT" : "!=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                default:
                    throw new NotSupportedException($"The expression type '{condition.ExpressionType}' was not supported.");
            }
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
    }
}
