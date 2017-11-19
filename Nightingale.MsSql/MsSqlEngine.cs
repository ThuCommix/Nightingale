using Nightingale.Queries;

namespace Nightingale.MsSql
{
    public class MsSqlEngine : ISqlGenerator
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
            if (limit != null && offset == null)
            {
                query = query.Insert(6, $" TOP {limit} ");
            }

            if (offset != null)
            {
                query = query.Insert(6, $" ROW_NUMBER() OVER (ORDER BY {tableAlias}.Id) AS _seq,");
                query = $"SELECT * FROM ({query}) t WHERE _seq BETWEEN {offset + 1} AND {offset + limit}";
            }
        }
    }
}
