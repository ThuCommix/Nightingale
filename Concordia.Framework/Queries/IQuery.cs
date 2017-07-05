using System;
using System.Collections.Generic;

namespace Concordia.Framework.Queries
{
    public interface IQuery
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        string Command { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        IEnumerable<QueryParameter> Parameters { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Gets the maximum results.
        /// </summary>
        int? MaxResults { get; }
    }
}
