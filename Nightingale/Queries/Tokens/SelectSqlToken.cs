using System.Linq;
using Nightingale.Metadata;

namespace Nightingale.Queries.Tokens
{
    public class SelectSqlToken : SqlToken
    {
        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        public EntityMetadata EntityMetadata { get; }

        /// <summary>
        /// Gets the table alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Initializes a new SelectSqlToken class.
        /// </summary>
        /// <param name="entityMetadata">The entity metadata.</param>
        public SelectSqlToken(EntityMetadata entityMetadata) : base(SqlTokenType.Select)
        {
            EntityMetadata = entityMetadata;
            Alias = entityMetadata.Table.ToLower();
            Sql = $"SELECT {string.Join(", ", entityMetadata.Fields.Select(x => x.Name))} FROM {entityMetadata.Table} {Alias}";
        }
    }
}
