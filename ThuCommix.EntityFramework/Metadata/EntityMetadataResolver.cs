using System;
using System.Collections.Generic;
using System.Linq;
using ThuCommix.EntityFramework.Entities;

namespace ThuCommix.EntityFramework.Metadata
{
	public class EntityMetadataResolver : IEntityMetadataResolver
	{
        /// <summary>
        /// Gets the entity metadata collection.
        /// </summary>
	    public IEnumerable<EntityMetadata> EntityMetadata => _entityMetadata.Select(x => x.Value); 

		private readonly Dictionary<Type, EntityMetadata> _entityMetadata = new Dictionary<Type, EntityMetadata>();
        private readonly IEntityMetadataService EntityMetadataService = DependencyResolver.GetInstance<IEntityMetadataService>();

	    public EntityMetadataResolver()
	    {
	        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
	        var entityTypes = assemblies.SelectMany(x => x.GetTypes().Where(y =>y.BaseType == typeof (Entity) && !y.IsAbstract));

	        foreach (var entityType in entityTypes)
	        {
                _entityMetadata[entityType] = EntityMetadataService.GetEntityMetadata(entityType);
            }
	    }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        /// <param name="entityMetadata">The entity metadata.</param>
        /// <returns>Returns the type or null.</returns>
        public Type GetEntityType(EntityMetadata entityMetadata)
        {
            return _entityMetadata.FirstOrDefault(x => x.Value == entityMetadata).Key;
        }

		/// <summary>
		/// Gets the entity metadata from the global cache.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The entity metadata.</returns>
		public EntityMetadata GetEntityMetadata(Entity entity)
		{
            return GetEntityMetadata(entity.GetType());
		}

        /// <summary>
        /// Gets the entity metadata from the global cache.
        /// </summary>
        /// <param name="entityType">The entity type.</param>
        /// <returns>The entity metadata.</returns>
        public EntityMetadata GetEntityMetadata(Type entityType)
	    {
            if (!_entityMetadata.ContainsKey(entityType))
            {
                _entityMetadata[entityType] = EntityMetadataService.GetEntityMetadata(entityType);
            }

            return _entityMetadata[entityType];
        }
	}
}
