using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Nightingale.Entities;
using Nightingale.Metadata;

namespace Nightingale
{
    public class EntityService : IEntityService
    {
        protected IEntityMetadataResolver EntityMetadataResolver => DependencyResolver.GetInstance<IEntityMetadataResolver>();

        /// <summary>
        /// Gets the child entities which would be affected by the specified cascade.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cascade">The cascade.</param>
        /// <returns>Returns a list of entities.</returns>
        public List<Entity> GetChildEntities(Entity entity, Cascade cascade)
        {
            var entities = new List<Entity>();
            GetChildEntities(entity, cascade, entities);

            return entities;
        }

        /// <summary>
        /// Creates the entity based on the specified data reader and type.
        /// </summary>
        /// <param name="reader">The data reader.</param>
        /// <param name="entityType">The entity type.</param>
        /// <returns>Returns the entity.</returns>
        public Entity CreateEntity(IDataReader reader, Type entityType)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            if(entityType == null)
                throw new ArgumentNullException(nameof(entityType));

            var entity = (Entity)Activator.CreateInstance(entityType);
            entity.PropertyChangeTracker.DisableChangeTracking = true;

            var metadata = EntityMetadataResolver.GetEntityMetadata(entity);

            foreach (var field in metadata.Fields)
            {
                var dbValue = reader[field.Name];
                if (dbValue == null || dbValue is DBNull)
                    continue;

                var propertyInfo = ReflectionHelper.GetProperty(entityType, field.Name);
                var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                if(field.Enum)
                {
                    var dbValueConverted = Convert.ToInt32(dbValue);
                    propertyInfo.SetValue(entity, Enum.ToObject(propertyType, dbValueConverted));
                }
                else
                {
                    var dbValueConverted = Convert.ChangeType(dbValue, propertyType);
                    propertyInfo.SetValue(entity, dbValueConverted);
                }
            }

            // enable tracking after the values are filled from the dataprovider.
            entity.PropertyChangeTracker.DisableChangeTracking = false;

            return entity;
        }

        /// <summary>
        /// Gets the child entities which would be affected by the specified cascade.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="cascade">The cascade,</param>
        /// <param name="entities">The entitylist.</param>
        private void GetChildEntities(Entity entity, Cascade cascade, List<Entity> entities)
        {
            if (entities.Contains(entity))
            {
                return;
            }

            entities.Add(entity);

            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);

            foreach (var field in metadata.Fields.Where(x => x.IsComplexFieldType && x.Cascade >= cascade))
            {
                try
                {
                    var result = ReflectionHelper.GetProperty(entityType, field.ForeignKey).GetValue(entity);
                    if (result != null)
                    {
                        var subEntity = (Entity)result;
                        GetChildEntities(subEntity, cascade, entities);
                    }
                }
                catch (TargetInvocationException)
                {
                }
            }

            foreach (var listField in metadata.ListFields.Where(x => x.Cascade >= cascade))
            {
                var entityCollection = (IEntityCollection)ReflectionHelper.GetProperty(entityType, listField.Name).GetValue(entity);
                var subEntities = entityCollection.GetCollectionItems();
                var removedEntities = entityCollection.GetRemovedCollectionItems();

                foreach (var subEntity in subEntities)
                {
                    if (subEntity != null)
                    {
                        GetChildEntities(subEntity, cascade, entities);
                    }
                }

                foreach (var subEntity in removedEntities)
                {
                    if (subEntity != null)
                    {
                        GetChildEntities(subEntity, cascade, entities);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the foreign fields.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void UpdateForeignFields(Entity entity)
        {
            var entityType = entity.GetType();
            var metadata = EntityMetadataResolver.GetEntityMetadata(entityType);
            foreach(var field in metadata.Fields.Where(x => x.IsForeignKey))
            {
                var propertyValue = ((Entity)ReflectionHelper.GetProperty(entityType, field.ForeignKey).GetValue(entity))?.Id;
                if (propertyValue == null)
                    continue;

                ReflectionHelper.GetProperty(entityType, field.Name).SetValue(entity, propertyValue.Value);
            }

            foreach(var listField in metadata.ListFields)
            {
                var propertyValue = (IEntityCollection)ReflectionHelper.GetProperty(entityType, listField.Name).GetValue(entity);
                foreach(var item in propertyValue.GetCollectionItems())
                {
                    ReflectionHelper.GetProperty(item.GetType(), listField.ReferenceField).SetValue(item, entity.Id);
                }
            }
        }
    }
}
