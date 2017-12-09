using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nightingale.Entities;
using Nightingale.Extensions;
using Nightingale.Metadata;

namespace Nightingale.Queries
{
    internal class QueryExpressionVisitor : ExpressionVisitor
    {
        /// <summary>
        /// Gets the joins.
        /// </summary>
        public IList<TableJoin> Joins { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IList<QueryParameter> Parameters { get; }

        /// <summary>
        /// Gets the includes.
        /// </summary>
        public IList<string> Includes { get; }

        /// <summary>
        /// Gets or sets the join table alias index.
        /// </summary>
        public int JoinTableAliasIndex { get; set; }

        /// <summary>
        /// Gets or sets the parameter alias index.
        /// </summary>
        public int ParameterAliasIndex { get; set; }

        /// <summary>
        /// Gets or sets the root table alias.
        /// </summary>
        public string RootTableAlias { get; set; }

        /// <summary>
        /// Gets or sets the parameter alias.
        /// </summary>
        public string ParameterAlias { get; set; }

        /// <summary>
        /// Gets or sets the join table alias.
        /// </summary>
        public string JoinTableAlias { get; set; }

        /// <summary>
        /// Gets the partial command.
        /// </summary>
        public string Command => _stringBuilder.ToString();

        /// <summary>
        /// Gets the limit.
        /// </summary>
        public int? Limit { get; private set; }

        /// <summary>
        /// Gets the offset.
        /// </summary>
        public int? Offset { get; private set; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        public Type ChangedEntityType { get; private set; }

        /// <summary>
        /// Gets the entity metadata resolver.
        /// </summary>
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        private EntityMetadata _currentEntityMetadata;
        private FieldMetadata _currentFieldMetadata;
        private string _currentAlias = "e";
        private string _propertyPath;
        private int _propertyDepth;
        private bool _hasWhere;
        private int _subQueryIndex;
        private bool _isOrdering;
        private string _orderProperty;
        private string _orderBy;
        private EntityMetadata _sourceEntityMetadata;
        private readonly Dictionary<string, TableJoin> _propertyJoinMapping;
        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new QueryExpressionVisitor class.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        public QueryExpressionVisitor(Type entityType)
        {
            Joins = new List<TableJoin>();
            Parameters = new List<QueryParameter>();
            Includes = new List<string>();

            if (entityType != typeof(Entity))
            {
                _currentEntityMetadata = EntityMetadataResolver.GetEntityMetadata(entityType);
                _sourceEntityMetadata = _currentEntityMetadata;
            }
 
            _propertyJoinMapping = new Dictionary<string, TableJoin>();
            _stringBuilder = new StringBuilder();
        }

        /// <summary>
        /// Initializes a new QueryExpressionVisitor class.
        /// </summary>
        /// <param name="entityMetadata">The entity meta data.</param>
        public QueryExpressionVisitor(EntityMetadata entityMetadata)
        {
            Joins = new List<TableJoin>();
            Parameters = new List<QueryParameter>();
            Includes = new List<string>();

            _currentEntityMetadata = entityMetadata;
            _sourceEntityMetadata = entityMetadata;
            _propertyJoinMapping = new Dictionary<string, TableJoin>();
            _stringBuilder = new StringBuilder();
        }

        public string ParseQueryExpression(Expression expression)
        {
            Visit(expression);
            if (Offset != null && Limit == null)
                Limit = int.MaxValue;

            var command = "SELECT " + string.Join(",", _sourceEntityMetadata.Fields.Select(x => $"{RootTableAlias}.{x.Name}")) + $" FROM {_sourceEntityMetadata.Table} {RootTableAlias}" +
                          (Joins.Count > 0 ? " " : string.Empty) + string.Join(" ", Joins.Select(x => x.ToString())) + $" {_stringBuilder} {_orderBy}";

            SqlGeneratorManager.SqlEngine.ApplyLimit(ref command, RootTableAlias, Limit, Offset);

            return command;
        }

        public string ParseCountQueryExpression(Expression expression)
        {
            Visit(expression);
            if (Offset != null && Limit == null)
                Limit = int.MaxValue;

            var command = $"SELECT COUNT({RootTableAlias}.Id) FROM {_sourceEntityMetadata.Table} {RootTableAlias}" + (Joins.Count > 0 ? " " : string.Empty) +
                   string.Join(" ", Joins.Select(x => x.ToString())) + $" {_stringBuilder} {_orderBy}";

            SqlGeneratorManager.SqlEngine.ApplyLimit(ref command, RootTableAlias, Limit, Offset);

            return command;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            EmitSql("(");
            Visit(node.Left);

            if (ResolveRightConstant(node.Right, out var constantValue))
            {
                EmitSql(" " + QueryHelper.GetOperatorSymbol(QueryHelper.ConvertOperator(node.NodeType), constantValue == null) + " ");
                Parameters.Add(GetQueryParameter($"@{ParameterAlias}{ParameterAliasIndex++}", constantValue, _currentFieldMetadata));
                EmitSql(Parameters.Last().Name);
                _currentAlias = RootTableAlias;
            }
            else
            {
                EmitSql(" " + QueryHelper.GetOperatorSymbol(QueryHelper.ConvertOperator(node.NodeType)) + " ");
                Visit(node.Right);
            }

            EmitSql(")");

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _propertyDepth++;
            var result = base.VisitMember(node);

            _currentFieldMetadata = _currentEntityMetadata.Fields.FirstOrDefault(x => x.Name == node.Member.Name || x.Name == $"FK_{node.Member.Name}_ID");
            if(_currentFieldMetadata == null)
                throw new InvalidOperationException("The property is not part of entity.");

            if (_currentFieldMetadata.IsComplexFieldType && !_currentFieldMetadata.Enum && _propertyDepth > 1)
            {
                _currentEntityMetadata = EntityMetadataResolver.EntityMetadata.FirstOrDefault(x => x.Name == _currentFieldMetadata.FieldType);
                _propertyPath = string.IsNullOrWhiteSpace(_propertyPath) ? node.Member.Name : $"{_propertyPath}.{node.Member.Name}";

                TableJoin tableJoin;

                //check if we already have a join
                if (_propertyJoinMapping.ContainsKey(_propertyPath))
                {
                    tableJoin = _propertyJoinMapping[_propertyPath];
                }
                else
                {
                    tableJoin = new TableJoin(_currentEntityMetadata.Table, GetTableAlias(), _currentAlias, _currentFieldMetadata.Name, !_currentFieldMetadata.Mandatory);
                    _propertyJoinMapping.Add(_propertyPath, tableJoin);

                    Joins.Add(tableJoin);
                }

                _currentAlias = tableJoin.Alias;
                _propertyDepth--;
            }
            else
            {
                if (!_isOrdering)
                {
                    EmitSql($"{_currentAlias}.{_currentFieldMetadata.Name}");
                }
                else
                {
                    _orderProperty = _currentFieldMetadata.Name;
                }

                _propertyPath = string.Empty;
                _propertyDepth = 0;
            }

            return result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "Where" || node.Method.Name == "FirstOrDefault" || node.Method.Name == "First" || node.Method.Name == "Single" || node.Method.Name == "SingleOrDefault")
            {
                if (node.Method.Name == "FirstOrDefault" || node.Method.Name == "First")
                    Limit = 1;

                Visit(node.Arguments[0]);
                if (node.Arguments.Count < 2)
                    return node;

                if (!_hasWhere)
                {
                    _hasWhere = true;
                    EmitSql("WHERE ");
                }
                else
                {
                    EmitSql(" AND ");
                    _currentEntityMetadata = _sourceEntityMetadata;
                }

                Visit(node.Arguments[1]);

                return node;
            }

            if (node.Method.Name == "Any" || node.Method.Name == "All" || node.Method.Name == "Count")
            {
                if (!(node.Arguments[0] is MemberExpression))
                {
                    Visit(node.Arguments[0]);
                    if (node.Arguments.Count == 1)
                        return node;

                    if (!_hasWhere)
                    {
                        _hasWhere = true;
                        EmitSql("WHERE ");
                    }
                    else
                    {
                        EmitSql(" AND ");
                    }
                    Visit(node.Arguments[1]);
                    return node;
                }

                var fieldMetadata = (FieldBaseMetadata)_currentEntityMetadata.Fields.FirstOrDefault(x => x.Name == (node.Arguments[0] as MemberExpression).Member.Name) ??
                                    _currentEntityMetadata.ListFields.FirstOrDefault(x => x.Name == (node.Arguments[0] as MemberExpression).Member.Name);

                string command = null;
                IList<TableJoin> joins = null;

                if (node.Arguments.Count == 2)
                {
                    _subQueryIndex++;
                    var qev = new QueryExpressionVisitor(EntityMetadataResolver.EntityMetadata.FirstOrDefault(x => x.Name == fieldMetadata.FieldType))
                    {
                        RootTableAlias = $"{RootTableAlias}{_subQueryIndex}",
                        ParameterAlias = ParameterAlias,
                        ParameterAliasIndex = ParameterAliasIndex,
                        JoinTableAlias = JoinTableAlias,
                        JoinTableAliasIndex = JoinTableAliasIndex
                    };
                    qev._currentAlias = qev.RootTableAlias;
                    //TODO needs beautify

                    qev.Visit(node.Arguments[1]);
                    ParameterAliasIndex = qev.ParameterAliasIndex;

                    foreach (var queryParameter in qev.Parameters)
                    {
                        Parameters.Add(queryParameter);
                    }

                    command = qev.Command;
                    joins = qev.Joins;
                }

                var referenceField = ((ListFieldMetadata) fieldMetadata).ReferenceField;
                if (node.Method.Name == "Any")
                {
                    EmitSql(string.IsNullOrWhiteSpace(command)
                        ? $"(SELECT COUNT({RootTableAlias}{_subQueryIndex}.Id) FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} WHERE {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id) > 0"
                        : $"{_currentAlias}.Id IN (SELECT {RootTableAlias}{_subQueryIndex}.{referenceField} FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} {string.Join(Environment.NewLine, joins.Select(x => x.ToString()))} WHERE {command} AND {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id)");
                }
                else if (node.Method.Name == "Count")
                {
                    EmitSql(string.IsNullOrWhiteSpace(command)
                        ? $"(SELECT COUNT({RootTableAlias}{_subQueryIndex}.Id) FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} WHERE {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id)"
                        : $"(SELECT COUNT({RootTableAlias}{_subQueryIndex}.Id) FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} {string.Join(Environment.NewLine, joins.Select(x => x.ToString()))} WHERE {command} AND {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id)");
                }
                else
                {
                    EmitSql($"(SELECT COUNT({RootTableAlias}{_subQueryIndex}.Id) FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} {string.Join(Environment.NewLine, joins.Select(x => x.ToString()))} WHERE {command} AND {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id) = (SELECT COUNT({RootTableAlias}{_subQueryIndex}.Id) FROM {fieldMetadata.FieldType} {RootTableAlias}{_subQueryIndex} {string.Join(Environment.NewLine, joins.Select(x => x.ToString()))} WHERE {RootTableAlias}{_subQueryIndex}.{referenceField} = {_currentAlias}.Id)");
                }

                return node;
            }

            if(node.Method.Name == "OrderBy" || node.Method.Name == "ThenBy")
            {
                Visit(node.Arguments[0]);
                _isOrdering = true;
                Visit(node.Arguments[1]);
                _isOrdering = false;

                EmitOrderBy($"{_currentAlias}.{_orderProperty} asc");

                _orderProperty = null;
                _currentAlias = RootTableAlias;
                _currentEntityMetadata = _sourceEntityMetadata;
                return node;
            }

            if (node.Method.Name == "OrderByDescending" || node.Method.Name == "ThenByDescending")
            {
                Visit(node.Arguments[0]);
                _isOrdering = true;
                Visit(node.Arguments[1]);
                _isOrdering = false;

                EmitOrderBy($"{_currentAlias}.{_orderProperty} desc");

                _orderProperty = null;
                _currentAlias = RootTableAlias;
                _currentEntityMetadata = _sourceEntityMetadata;
                return node;
            }

            if (node.Method.Name == "StartsWith")
            {
                Visit(node.Object);

                var constantValue = DynamicInvokeExpression(node.Arguments[0]);
                EmitSql(" LIKE ");
                Parameters.Add(GetQueryParameter($"@{ParameterAlias}{ParameterAliasIndex++}", $"{constantValue}%", _currentFieldMetadata));
                EmitSql(Parameters.Last().Name);
                _currentAlias = RootTableAlias;
                return node;
            }

            if (node.Method.Name == "EndsWith")
            {
                Visit(node.Object);

                var constantValue = DynamicInvokeExpression(node.Arguments[0]);
                EmitSql(" LIKE ");
                Parameters.Add(GetQueryParameter($"@{ParameterAlias}{ParameterAliasIndex++}", $"%{constantValue}", _currentFieldMetadata));
                EmitSql(Parameters.Last().Name);
                _currentAlias = RootTableAlias;
                return node;
            }

            if (node.Method.Name == "Contains")
            {
                Visit(node.Object);

                var constantValue = DynamicInvokeExpression(node.Arguments[0]);
                EmitSql(" LIKE ");
                Parameters.Add(GetQueryParameter($"@{ParameterAlias}{ParameterAliasIndex++}", $"%{constantValue}%", _currentFieldMetadata));
                EmitSql(Parameters.Last().Name);
                _currentAlias = RootTableAlias;
                return node;
            }

            if (node.Method.Name == "Include")
            {
                Visit(node.Arguments[0]);

                Includes.Add(GetIncludeMember(node.Arguments[1]));

                return node;
            }

            if (node.Method.Name == "Take")
            {
                Limit = (int)DynamicInvokeExpression(node.Arguments[1]);
                return Visit(node.Arguments[0]);
            }

            if (node.Method.Name == "Skip")
            {
                Offset = (int)DynamicInvokeExpression(node.Arguments[1]);
                return Visit(node.Arguments[0]);
            }

            if (node.Method.Name == "ChangeQueryType")
            {
                var entityType = (node.Arguments[1] as ConstantExpression).Value as Type;
                _currentEntityMetadata = EntityMetadataResolver.GetEntityMetadata(entityType);
                _sourceEntityMetadata = _currentEntityMetadata;

                ChangedEntityType = entityType;

                return Visit(node.Arguments[0]);
            }

            throw new NotSupportedException($"{node.Method.Name} is not supported.");
        }

        private string GetIncludeMember(Expression expression)
        {
            if (expression is UnaryExpression unaryExpression)
            {
                if (unaryExpression.Operand is LambdaExpression lambdaExpression)
                {
                    return (lambdaExpression.Body as MemberExpression).Member.Name;
                }
            }

            throw new InvalidOperationException("Include could not be resolved.");
        }

        private bool ResolveRightConstant(Expression expression, out object value)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Lambda:
                    var lambdaExpression = expression as LambdaExpression;
                    return ResolveRightConstant(lambdaExpression.Body, out value);
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    value = Expression.Lambda(memberExpression).Compile().DynamicInvoke();
                    return true;
                case ExpressionType.Convert:
                    var unaryExpression = expression as UnaryExpression;
                    return ResolveRightConstant(unaryExpression.Operand, out value);
                case ExpressionType.Constant:
                    var constantExpression = expression as ConstantExpression;
                    value = constantExpression.Value;
                    return true;
            }

            value = null;
            return false;
        }

        private object DynamicInvokeExpression(Expression expression)
        {
            return Expression.Lambda(expression).Compile().DynamicInvoke();
        }

        private string GetTableAlias()
        {
            return $"{JoinTableAlias}{JoinTableAliasIndex++}";
        }

        private void EmitSql(string sql)
        {
            _stringBuilder.Append(sql);
        }

        private void EmitOrderBy(string sql)
        {
            if (string.IsNullOrWhiteSpace(_orderBy))
            {
                _orderBy = $"ORDER BY {sql}";
            }
            else
            {
                _orderBy += $", {sql}";
            }
        }

        /// <summary>
        /// Gets a query parameter based on the specified name, value and field metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="fieldMetadata">The field metadata.</param>
        /// <returns>Returns a QueryParameter instance.</returns>
        private static QueryParameter GetQueryParameter(string name, object value, FieldMetadata fieldMetadata)
        {
            if(fieldMetadata != null)
                return new QueryParameter(name, value, fieldMetadata.GetSqlDbType(), !fieldMetadata.Mandatory, fieldMetadata.MaxLength);

            return new QueryParameter(name, value, GetSqlDbTypeForClrType(value.GetType()), false);
        }

        private static SqlDbType GetSqlDbTypeForClrType(Type type)
        {
            if (type == typeof(string))
                return SqlDbType.VarChar;

            if (type == typeof(decimal) || type == typeof(decimal?))
                return SqlDbType.Decimal;

            if (type == typeof(bool) || type == typeof(bool?))
                return SqlDbType.Bit;

            if (type == typeof(DateTime) || type == typeof(DateTime?))
                return SqlDbType.DateTime;

            return SqlDbType.Int;
        }
    }
}
