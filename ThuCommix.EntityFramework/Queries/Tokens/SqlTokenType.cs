namespace ThuCommix.EntityFramework.Queries.Tokens
{
    public enum SqlTokenType
    {
        /// <summary>
        /// Select token type.
        /// </summary>
        Select,

        /// <summary>
        /// Join token type.
        /// </summary>
        Join,

        /// <summary>
        /// Condition token type.
        /// </summary>
        Condition,

        /// <summary>
        /// Condition link token type.
        /// </summary>
        ConditionLink
    }
}
