namespace Concordia.Framework.Queries.Tokens
{
    public abstract class SqlToken
    {
        /// <summary>
        /// Gets the sql token type.
        /// </summary>
        public SqlTokenType TokenType { get; }

        /// <summary>
        /// Gets the sql for this token.
        /// </summary>
        public string Sql { get; protected set; }

        /// <summary>
        /// Initializes a new SqlToken class.
        /// </summary>
        /// <param name="tokenType">The sql token type.</param>
        protected SqlToken(SqlTokenType tokenType)
        {
            TokenType = tokenType;
        }
    }
}
