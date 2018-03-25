using System;
using Nightingale.Metadata;

namespace Nightingale.Queries
{
    internal class SelectDescription
    {
        /// <summary>
        /// A value indicating whether the select description contains a full entity select.
        /// </summary>
        public bool IsFullEntity => Metadata != null;

        /// <summary>
        /// Gets the entity metadata.
        /// </summary>
        public EntityMetadata Metadata { get; }

        /// <summary>
        /// Gets the alias.
        /// </summary>
        public string Alias { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Initializes a new SelectDescription class.
        /// <param name="identifier">The identifier.</param>
        /// <param name="alias">The alias.</param>
        /// </summary>
        public SelectDescription(string identifier, string alias)
        {
            Identifier = identifier;
            Alias = alias;
        }

        /// <summary>
        /// Initializes a new SelectDescription class.
        /// </summary>
        /// <param name="metadata">The entity metadata.</param>
        /// <param name="identifier">The identifier.</param>
        /// <param name="alias">The alias.</param>
        public SelectDescription(EntityMetadata metadata, string identifier, string alias)
        {
            Metadata = metadata;
            Identifier = identifier;
            Alias = alias;
        }
    }
}
