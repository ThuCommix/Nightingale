namespace Nightingale.Queries
{
    public class AnsiSqlEngine : ISqlGenerator
    {
        /// <summary>
        /// Applies the limit clausel to the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="tableAlias">The table alias.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        public void ApplyLimit(ref string query, string tableAlias, int? limit, int? offset)
        {
            if (limit.HasValue)
            {
                query += $" LIMIT {limit}";
            }

            if (offset.HasValue)
            {
                query += $" OFFSET {offset}";
            }
        }
    }
}
