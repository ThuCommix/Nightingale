namespace Nightingale.Queries.Tokens
{
    internal class BinaryToken : Token
    {
        /// <summary>
        /// Gets the operator.
        /// </summary>
        public Operator Operator { get; }

        /// <summary>
        /// Initializes a new BinaryToken class.
        /// </summary>
        /// <param name="op">The op.</param>
        public BinaryToken(Operator op) : base(TokenType.Binary)
        {
            Operator = op;
        }
    }
}
