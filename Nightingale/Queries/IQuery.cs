using System;
using System.Collections.Generic;

namespace Nightingale.Queries
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
        IList<QueryParameter> Parameters { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        Type EntityType { get; }
    }
}
