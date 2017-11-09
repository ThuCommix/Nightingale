using System.Collections.Generic;

namespace Nightingale.Queries.Tokens
{
    public class TokenComposerResult
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IEnumerable<QueryParameter> Parameters { get; }

        /// <summary>
        /// Initializes a new TokenComposerResult class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The parameters.</param>
        public TokenComposerResult(string command, IEnumerable<QueryParameter> parameters)
        {
            Command = command;
            Parameters = parameters;
        }
    }
}
