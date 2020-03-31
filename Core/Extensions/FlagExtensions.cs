using System;
using System.Collections.Generic;

namespace Atlas.Core.Extensions
{
	public static class FlagExtensions
	{
		public static IEnumerable<T> GetFlags<T>(this T item, bool zero = false)
			where T : Enum
		{
			foreach(T value in Enum.GetValues(typeof(T)))
			{
				if(value.OneFlag(zero) && item.HasFlag(value))
					yield return value;
			}
		}

		public static T SetFlags<T>(this IEnumerable<T> items)
			where T : Enum
		{
			int flags = 0;
			foreach(var item in items)
				flags |= Convert.ToInt32(item);
			return (T)Enum.Parse(typeof(T), flags.ToString());
		}

		public static bool AnyFlags<T>(this T flag1, T flag2)
			where T : Enum
		{
			foreach(T value in Enum.GetValues(typeof(T)))
			{
				if(flag1.HasFlag(value) && flag2.HasFlag(value))
					return true;
			}
			return false;
		}

		public static bool OneFlag(this Enum item, bool zero = false)
		{
			int value = Convert.ToInt32(item);
			return (value != 0 || zero) && ((value & (value - 1)) == 0);
		}
	}
}