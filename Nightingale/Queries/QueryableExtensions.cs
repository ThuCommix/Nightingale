using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Nightingale.Entities;

namespace Nightingale.Queries
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Indicates which properties or lists should be eager-loaded.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="objSelector">The object selector.</param>
        /// <returns>Returns the IQueryable interface.</returns>
        public static IQueryable<T> Include<T>(this IQueryable<T> queryable, Expression<Func<T, object>> objSelector) where T : Entity
        {
            var methodInfo = typeof(QueryableExtensions).GetMethod("Include", BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(typeof(T));
            var method = Expression.Call(methodInfo, queryable.Expression, objSelector);
            return queryable.Provider.CreateQuery<T>(method);
        }

        /// <summary>
        /// Changes the query type.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="entityType">The new entity type.</param>
        /// <returns>Returns the IQueryable interface.</returns>s
        internal static IQueryable<T> ChangeQueryType<T>(this IQueryable<T> queryable, Type entityType)
        {
            var methodInfo = typeof(QueryableExtensions).GetMethod("ChangeQueryType", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(typeof(T));
            var method = Expression.Call(methodInfo, queryable.Expression, Expression.Constant(entityType));
            return queryable.Provider.CreateQuery<T>(method);
        }

        /// <summary>
        /// Applies the delete filter.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <returns>Returns the queryable with delete filter.</returns>
        public static IQueryable<T> ApplyDeleteFilter<T>(this IQueryable<T> queryable) where T : Entity
        {
            return queryable.Where(x => x.Deleted == false);
        }

        /// <summary>
        /// Applies the id filter.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="queryable">The queryable.</param>
        /// <param name="id"></param>
        /// <returns>Returns the queryable with id filter.</returns>
        public static IQueryable<T> ApplyIdFilter<T>(this IQueryable<T> queryable, int id) where T : Entity
        {
            return queryable.Where(x => x.Id == id);
        }
    }
}
