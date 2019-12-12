using System;
using System.Collections.Generic;

namespace Atlas.Core.Utilites
{
	public static class Collection
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