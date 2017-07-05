using System;
using System.Collections.Generic;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Metadata
{
    public interface IEntityMetadataResolver
    {
        IEnumerable<EntityMetadata> EntityMetadata { get; }

        Type GetEntityType(EntityMetadata entityMetadata);

        EntityMetadata GetEntityMetadata(Entity entity);

        EntityMetadata GetEntityMetadata(Type entityType);
    }
}
