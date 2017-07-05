using Concordia.Framework.Queries;

namespace Concordia.Framework.MsSql
{
    public static class QueryConverter
    {
        /// <summary>
        /// Gets the query command.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Returns the mssql converted query command.</returns>
        public static string GetQueryCommand(IQuery query)
        {
            if (query.MaxResults == null)
                return query.Command;

            var command = query.Command;
            var index = command.IndexOf("LIMIT");

            command = command.Substring(0, index);
            command = command.Insert(6, $" TOP {query.MaxResults} ");

            return command;
        }
    }
}
