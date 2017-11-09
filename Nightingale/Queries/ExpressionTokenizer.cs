using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Nightingale.Entities;
using Nightingale.Queries.Tokens;

namespace Nightingale.Queries
{
    internal class ExpressionTokenizer
    {
        /// <summary>
        /// Gets a stack of evaluated tokens.
        /// </summary>
        public List<Token> Tokens { get; }

        private string _memberPath;

        /// <summary>
        /// Initializes a new ExpressionTokenizer class.
        /// </summary>
        public ExpressionTokenizer()
        {
            Tokens = new List<Token>();
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="expression">The expression.</param>
        public void Eval<T>(Expression<Func<T, bool>> expression) where T : Entity
        {
            Eval((Expression)expression);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void Eval(Expression expression)
        {
            EvalInternal(expression);
        }

        /// <summary>
        /// Evaluates the expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void EvalInternal(Expression expression)
        {
            switch(expression.NodeType)
            {
                case ExpressionType.Lambda:
                    EvalInternal((expression as LambdaExpression).Body);
                    break;
                case ExpressionType.Call:
                    var methodExpression = expression as MethodCallExpression;
                    if (methodExpression.Object != null)
                    {
                        EvalInternal(methodExpression.Object);
                        Tokens.Add(new MethodToken(methodExpression.Method.Name, methodExpression.Method.DeclaringType));
                    }

                    for (int i = 0; i < methodExpression.Arguments.Count; i++)
                    {
                        if (i == 0 && methodExpression.Object == null)
                        {
                            EvalInternal(methodExpression.Arguments[0]);
                            Tokens.Add(new MethodToken(methodExpression.Method.Name, methodExpression.Method.DeclaringType));
                        }
                        else
                        {
                            EvalInternal(methodExpression.Arguments[i]);
                        }
                    }
                    break;
                case ExpressionType.MemberAccess:
                    var memberExpression = expression as MemberExpression;
                    if(memberExpression.Expression.NodeType == ExpressionType.Constant)
                    {
                        //here we get the property path in the member and this is the object being evaluated on
                        _memberPath = memberExpression.Member.Name;
                        EvalInternal(memberExpression.Expression);
                    }
                    else
                    {
                        EvalInternal(memberExpression.Expression);
                        if(typeof(Entity).IsAssignableFrom(memberExpression.Member.DeclaringType))
                            Tokens.Add(new PropertyToken(memberExpression.Member.Name));
                    }
                    break;
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.And:
                case ExpressionType.Or:
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    EvalInternal((expression as BinaryExpression).Left);
                    Tokens.Add(new BinaryToken(ConvertExpressionType(expression.NodeType)));
                    EvalInternal((expression as BinaryExpression).Right);
                    break;
                case ExpressionType.Convert:
                    var unaryExpression = expression as UnaryExpression;
                    EvalInternal(unaryExpression.Operand);
                    break;
                case ExpressionType.Constant:
                    var constantExpression = expression as ConstantExpression;
                    if (!string.IsNullOrWhiteSpace(_memberPath))
                    {
                        var type = constantExpression.Value.GetType();
                        var property = type.GetProperty(_memberPath);

                        Tokens.Add(property != null
                            ? new ConstantToken(property.GetValue(constantExpression.Value))
                            : new ConstantToken(type.GetField(_memberPath).GetValue(constantExpression.Value)));
                    }
                    else
                    {
                        Tokens.Add(new ConstantToken(constantExpression.Value));
                    }

                    _memberPath = null;
                    break;
                case ExpressionType.Parameter:
                    var parameterExpression = expression as ParameterExpression;
                    Tokens.Add(new TypeContextToken(parameterExpression.Type));
                    break;
                default:
                    throw new NotSupportedException($"The nodeType {expression.NodeType} was not supported.");
            }
        }

        private static Operator ConvertExpressionType(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                case ExpressionType.Equal:
                    return Operator.Equal;
                case ExpressionType.NotEqual:
                    return Operator.NotEqual;
                case ExpressionType.GreaterThan:
                    return Operator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Operator.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return Operator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Operator.LessThanOrEqual;
                case ExpressionType.AndAlso:
                    return Operator.AndAlso;
                case ExpressionType.OrElse:
                    return Operator.OrElse;
                default:
                    throw new NotSupportedException($"The expression type '{expressionType}' was not supported.");
            }
        }
    }
}
