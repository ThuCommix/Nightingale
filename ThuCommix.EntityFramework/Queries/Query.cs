using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<QueryParameter> Parameters { get; private set; }

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

        private readonly List<QueryConditionGroup> _groups;
        private string _command;

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        public Query()
        {
            _groups = new List<QueryConditionGroup>();
            Parameters = new List<QueryParameter>();
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
                Parameters = parameters;
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

        private void CompileSql()
        {
            var entityMetadataResolver = DependencyResolver.GetInstance<IEntityMetadataResolver>();
            var metadata = entityMetadataResolver.GetEntityMetadata(EntityType);
            var commandBuilder = new StringBuilder();
            var entityName = EntityType.Name.ToLower();
            var parameters = new List<QueryParameter>();
            var joins = new List<Tuple<string, string>>();
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

                    foreach (var propertyName in propertyList)
                    {
                        currentPropertyPath += currentPropertyPath == string.Empty ? propertyName : $".{propertyName}";
                        var fieldMetadata = currentMetadata.Fields.FirstOrDefault(x => x.Name == propertyName || x.Name == $"FK_{propertyName}_ID");
                        if (!fieldMetadata.IsComplexFieldType && !fieldMetadata.IsForeignKey)
                        {
                            conditionCommands.Add($"{previousAlias}.{fieldMetadata.Name} = {parameterName}");
                        }
                        else
                        {
                            if (propertyList.Length == 1)
                            {
                                conditionCommands.Add($"{previousAlias}.{fieldMetadata.Name} = {parameterName}");
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

                    parameters.Add(new QueryParameter(parameterName, equationValue));
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

            if(MaxResults != null)
            {
                commandBuilder.AppendLine($"LIMIT {MaxResults}");
            }

            _command = commandBuilder.ToString().Replace(Environment.NewLine, " ");
            Parameters = parameters;
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

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns a query.</returns>
        public static Query CreateQuery<T>() where T : Entity
        {
            return new Query(typeof(T));
        }
    }
}
