using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.Core.Extensions;

public static class FlagExtensions
{
	#region Static
	public static IEnumerable<T> GetFlags<T>(bool includeZero = false) where T : struct, Enum
	{
		return Enum.GetValues<T>().Where(v => v.OneFlag(includeZero));
	}

	public static bool OneFlag(int value, bool includeZero = false) => (value != 0 || includeZero) && ((value & (value - 1)) == 0);
	#endregion

	#region Extensions
	public static IEnumerable<T> GetFlags<T>(this T flags, bool includeZero = false)
		where T : struct, Enum
	{
		return GetFlags<T>(includeZero).Where(v => flags.HasFlag(v));
	}

	public static T SetFlags<T>(this IEnumerable<T> flags)
		where T : struct, Enum
	{
		return (T)(object)flags.Select(ToInt).Aggregate((flag1, flag2) => flag1 | flag2);
	}

	public static bool AnyFlags<T>(this T flags1, T flags2, bool includeZero = false)
		where T : struct, Enum
	{
		return GetFlags<T>(includeZero).Any(f => flags1.HasFlag(f) && flags2.HasFlag(f));
	}

	public static bool OneFlag<T>(this T flags, bool includeZero = false)
		where T : struct, Enum
	{
		return OneFlag(flags.ToInt(), includeZero);
	}

	public static int ToInt<T>(this T flags) where T : struct, Enum => Convert.ToInt32(flags);
	#endregion
}