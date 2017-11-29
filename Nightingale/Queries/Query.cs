using System;
using System.Collections.Generic;
using Nightingale.Entities;

namespace Nightingale.Queries
{
    public class Query : IQuery
    {
        /// <summary>
        /// Gets the command.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        public IList<QueryParameter> Parameters { get; }

        /// <summary>
        /// Gets or sets the entity type.
        /// </summary>
        public Type EntityType { get; set; }

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        public Query()
        {
            Parameters = new List<QueryParameter>();
        }

        /// <summary>
        /// Initializes a new Query class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="entityType">The entity type.</param>
        /// <param name="parameters">The query parameters.</param>
        public Query(string command, Type entityType, IEnumerable<QueryParameter> parameters = null)
        {
            Command = command;
            EntityType = entityType;
            Parameters = parameters == null ? new List<QueryParameter>() : new List<QueryParameter>(parameters);
        }

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <returns>Returns a new query.</returns>
        public static Query CreateQuery<T>() where T : Entity
        {
            return new Query(null, typeof(T));
        }

        /// <summary>
        /// Creates a new query.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="command">The command.</param>
        /// <param name="parameters">The query parameters.</param>
        /// <returns>Returns a new query.</returns>
        public static Query CreateQuery<T>(string command, IEnumerable<QueryParameter> parameters = null) where T : Entity
        {
            return new Query(command, typeof(T), parameters);
        }
    }
}
