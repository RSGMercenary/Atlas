using System;
using System.Collections.Generic;

namespace Atlas.Core.Extensions
{
	public static class CollectionExtensions
	{
		#region Enumerables
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> items, Action<T> action)
		{
			foreach(var item in items)
			{
				action.Invoke(item);
				yield return item;
			}
		}
		#endregion

		#region Lists
		public static T Pop<T>(this IList<T> list)
		{
			var index = list.Count - 1;
			var value = list[index];
			list.RemoveAt(index);
			return value;
		}
		#endregion
	}
}