namespace Atlas.Tests.ECS.Serialization;

static class RandomExtensions
{
	public static bool NextBool(this Random random) => random.NextDouble() >= 0.5;

}