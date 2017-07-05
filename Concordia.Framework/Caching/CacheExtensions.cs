using System;
using Concordia.Framework.Entities;

namespace Concordia.Framework.Caching
{
    public static class CacheExtensions
    {
        public static void Remove(this ICache cache, Entity entity)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            cache.Remove(entity.Id, entity.GetType());
        }

        public static T Get<T>(this ICache cache, int id) where T : Entity
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            return cache.Get(id, typeof(T)) as T;
        }

        public static bool TryGet<T>(this ICache cache, int id, out T entity) where T : Entity
        {
            entity = null;
            Entity genericEntity;

            if (TryGet(cache, id, typeof(T), out genericEntity))
            {
                entity = genericEntity as T;
            }

            return entity != null;
        }

        public static bool TryGet(this ICache cache, int id, Type entityType, out Entity entity)
        {
            if (cache == null)
                throw new ArgumentNullException(nameof(cache));

            entity = cache.Get(id, entityType);
            return entity != null;
        }
    }
}
