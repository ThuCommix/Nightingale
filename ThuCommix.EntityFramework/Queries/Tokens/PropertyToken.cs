namespace ThuCommix.EntityFramework.Queries.Tokens
{
    public class PropertyToken : Token
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Initializes a new PropertyToken class.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        public PropertyToken(string propertyName) : base(TokenType.Property)
        {
            Property = propertyName;
        }
    }
}
