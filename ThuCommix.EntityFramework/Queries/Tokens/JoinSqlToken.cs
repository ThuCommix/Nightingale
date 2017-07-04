using ThuCommix.EntityFramework.Metadata;

namespace ThuCommix.EntityFramework.Queries.Tokens
{
    internal class JoinSqlToken : SqlToken
    {
        /// <summary>
        /// The source entity metadata.
        /// </summary>
        public EntityMetadata SourceEntityMetadata { get; }

        /// <summary>
        /// The target entity metadata.
        /// </summary>
        public EntityMetadata TargetEntityMetadata { get; }

        /// <summary>
        /// The navigation field metadata.
        /// </summary>
        public FieldMetadata NavigationFieldMetadata { get; }

        /// <summary>
        /// Gets the target alias.
        /// </summary>
        public string TargetAlias { get; }

        /// <summary>
        /// Gets the source alias.
        /// </summary>
        public string SourceAlias { get; }

        /// <summary>
        /// Initializes a new JoinSqlToken class.
        /// </summary>
        /// <param name="aliasIndex">The alias index.</param>
        /// <param name="sourceAlias">The source alias.</param>
        /// <param name="sourceEntityMetadata">The source entity metadata.</param>
        /// <param name="fieldMetadata">The navigation field metadata.</param>
        /// <param name="targetEntityMetadata">The target entity metadata.</param>
        public JoinSqlToken(int aliasIndex, string sourceAlias, EntityMetadata sourceEntityMetadata, FieldMetadata fieldMetadata, EntityMetadata targetEntityMetadata) : base(SqlTokenType.Join)
        {
            SourceEntityMetadata = sourceEntityMetadata;
            TargetEntityMetadata = targetEntityMetadata;
            NavigationFieldMetadata = fieldMetadata;
            SourceAlias = sourceAlias;
            TargetAlias = $"a{aliasIndex}";
            Sql = $"{GetJoinType(fieldMetadata.Mandatory)} JOIN {targetEntityMetadata.Table} {TargetAlias} ON {TargetAlias}.Id = {sourceAlias}.{fieldMetadata.Name}";
        }

        /// <summary>
        /// Gets the join type based on the specified mandatory flag.
        /// </summary>
        /// <param name="mandatory">The mandatory state.</param>
        /// <returns>Returns either INNER or LEFT based on the mandatory flag.</returns>
        private static string GetJoinType(bool mandatory)
        {
            return mandatory ? "INNER" : "LEFT";
        }
    }
}
