namespace Nightingale.Queries.Tokens
{
    public abstract class Token
    {
        /// <summary>
        /// Gets the token type.
        /// </summary>
        public TokenType TokenType { get; }

        /// <summary>
        /// Initializes a new Token class.
        /// </summary>
        /// <param name="tokenType">The token type.</param>
        protected Token(TokenType tokenType)
        {
            TokenType = tokenType;
        }
    }
}
