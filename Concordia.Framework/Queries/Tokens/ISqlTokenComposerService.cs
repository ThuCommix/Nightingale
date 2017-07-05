using System.Collections.Generic;

namespace Concordia.Framework.Queries.Tokens
{
    public interface ISqlTokenComposerService
    {
        /// <summary>
        /// Composes the sql based on the specified tokens.
        /// </summary>
        /// <param name="sqlTokens">The sql tokens.</param>
        /// <returns>Returns the composed sql.</returns>
        TokenComposerResult ComposeSql(IEnumerable<SqlToken> sqlTokens);
    }
}
