using System;
using System.Collections.Generic;
using System.Linq;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Metadata
{
	public static class EntityMetadataResolver
	{
        /// <summary>
        /// Gets the entity metadata collection.
        /// </summary>
	    public static IEnumerable<EntityMetadata> EntityMetadataList => EntityMetadata.Select(x => x.Value); 

		private static readonly Dictionary<Type, EntityMetadata> EntityMetadata = new Dictionary<Type, EntityMetadata>();
		private static readonly IEntityMetadataService EntityMetadataService = new EntityMetadataService();

	    static EntityMetadataResolver()
	    {
	        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
	        var entityTypes = assemblies.SelectMany(x => x.GetTypes().Where(y =>y.BaseType == typeof (Entity) && !y.IsAbstract));

	        foreach (var entityType in entityTypes)
	        {
                EntityMetadata[entityType] = EntityMetadataService.GetEntityMetadata(entityType);
            }
	    }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        /// <param name="entityMetadata">The entity metadata.</param>
        /// <returns>Returns the type or null.</returns>
        public static Type GetEntityType(EntityMetadata entityMetadata)
        {
            return EntityMetadata.FirstOrDefault(x => x.Value == entityMetadata).Key;
        }

		/// <summary>
		/// Gets the entity metadata from the global cache.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The entity metadata.</returns>
		public static EntityMetadata GetEntityMetadata(Entity entity)
		{
            return GetEntityMetadata(entity.GetType());
		}

        /// <summary>
        /// Gets the entity metadata from the global cache.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        public static EntityMetadata GetEntityMetadata(Type entityType)
	    {
            if (!EntityMetadata.ContainsKey(entityType))
            {
                EntityMetadata[entityType] = EntityMetadataService.GetEntityMetadata(entityType);
            }

            return EntityMetadata[entityType];
        }
	}
}
