using System;
using System.Collections.Generic;
using System.Data;
using Nightingale.Entities;

namespace Nightingale
{
    public interface IEntityService
    {
        /// <summary>
        /// Gets the child entities which would be affected by the specified cascade.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cascade">The cascade.</param>
        /// <returns>Returns a list of entities.</returns>
        List<Entity> GetChildEntities(Entity entity, Cascade cascade);

        /// <summary>
        /// Creates the entity based on the specified data reader and type.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity.</returns>
        Entity CreateEntity(IDataReader reader, Type entityType);

        /// <summary>
        /// Updates the foreign fields.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void UpdateForeignFields(Entity entity);
    }
}
