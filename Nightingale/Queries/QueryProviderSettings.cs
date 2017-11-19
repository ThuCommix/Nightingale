namespace Nightingale.Queries
{
    public class QueryProviderSettings
    {
        /// <summary>
        /// Gets or sets the root table alias.
        /// </summary>
        public string RootTableAlias { get; set; } = "e";

        /// <summary>
        /// Gets or sets the parameter alias.
        /// </summary>
        public string ParameterAlias { get; set; } = "p";

        /// <summary>
        /// Gets or sets the parameter alias start index.
        /// </summary>
        public int ParameterAliasStartIndex { get; set; } = 0;

        /// <summary>
        /// Gets or sets the join table alias.
        /// </summary>
        public string JoinTableAlias { get; set; } = "v";

        /// <summary>
        /// Gets or sets the join table alias start index.
        /// </summary>
        public int JoinTableAliasStartIndex { get; set; } = 0;
    }
}
