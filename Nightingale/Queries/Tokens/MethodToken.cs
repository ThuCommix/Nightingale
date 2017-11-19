using System;

namespace Nightingale.Queries.Tokens
{
    internal class MethodToken : Token
    {
        /// <summary>
        /// Gets the method name.
        /// </summary>
        public string MethodName { get; }

        /// <summary>
        /// Gets the declaring type.
        /// </summary>
        public Type DeclaringType { get; }

        /// <summary>
        /// Initializes a new MethodToken class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="declaringType">The declaring type.</param>
        public MethodToken(string name, Type declaringType) : base(TokenType.Method)
        {
            MethodName = name;
            DeclaringType = declaringType;
        }
    }
}
