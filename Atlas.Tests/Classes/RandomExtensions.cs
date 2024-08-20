using System;

namespace Atlas.Tests.Classes;

static class RandomExtensions
{
	public static bool NextBool(this Random random) => random.NextDouble() >= 0.5;

	public static int NextExclude(this Random random, int maxValue, int exclude)
	{
		var value = random.Next(maxValue);
		while(value == exclude)
			value = random.Next(maxValue);
		return value;
	}
}