namespace Nightingale.Queries.Tokens
{
    public class ConstantToken : Token
    {
        /// <summary>
        /// Gets the constant value.
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Initializes a new ConstantToken class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ConstantToken(object value) : base(TokenType.Constant)
        {
            Value = value;
        }
    }
}
