using System;
using System.Collections.Generic;

namespace Atlas.Core.Extensions;

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

	public static IEnumerable<T> ToEnumerable<T>(this T item)
	{
		yield return item;
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

	public static void Swap<T>(this IList<T> list, int index1, int index2)
	{
		var value = list[index2];
		list[index2] = list[index1];
		list[index1] = value;
	}
	#endregion

	#region Dictionaries
	public static bool TryGetValue<TKey, TValue, T>(this IDictionary<TKey, TValue> dictionary, TKey key, out T cast)
		where T : TValue
	{
		if(dictionary.TryGetValue(key, out var value))
		{
			cast = (T)value;
			return true;
		}
		cast = default;
		return false;
	}
	#endregion
}