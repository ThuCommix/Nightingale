namespace Nightingale.Queries
{
    public interface ISqlGenerator
    {
        /// <summary>
        /// Applies the limit clausel to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        void ApplyLimit(ref string query, string tableAlias, int? limit, int? offset);
    }
}
