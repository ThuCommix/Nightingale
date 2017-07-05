using System;
using System.Collections.Generic;

namespace Concordia.Framework.Extensions
{
	public static class EnumerableExtension
	{
		/// <summary>
		/// Invokes an action for each collection item.
		/// </summary>
		/// <param name="collection">The collection.</param>
		/// <param name="action">The action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
		{
			foreach(var item in collection)
			{
				action.Invoke(item);
			}
		}
	}
}
