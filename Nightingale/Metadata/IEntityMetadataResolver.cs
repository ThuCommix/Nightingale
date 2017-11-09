using System;
using System.Collections.Generic;
using Nightingale.Entities;

namespace Nightingale.Metadata
{
    public interface IEntityMetadataResolver
    {
        IEnumerable<EntityMetadata> EntityMetadata { get; }

        Type GetEntityType(EntityMetadata entityMetadata);

        EntityMetadata GetEntityMetadata(Entity entity);

        EntityMetadata GetEntityMetadata(Type entityType);
    }
}
