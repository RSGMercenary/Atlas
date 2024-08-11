using Atlas.Core.Extensions;
using NUnit.Framework;

namespace Atlas.Tests.Core.Extensions;

[TestFixture]
internal class FlagExtensionTests
{
	[TestCase(true)]
	[TestCase(false)]
	public void When_GetFlags_FromEnum_Then_OneFlag(bool includeZero)
	{
		FlagExtensions.GetFlags<TestFlags>(includeZero).ForEach(f => Assert.That(f.OneFlag(includeZero)));
	}

	[TestCase(TestFlags.None | TestFlags.C | TestFlags.L | TestFlags.O, 5, true)]
	[TestCase(TestFlags.A | TestFlags.B | TestFlags.K, 3, false)]
	public void When_GetFlags_FromFlags_Then_OneFlag(TestFlags flags, int count, bool includeZero)
	{
		Assert.That(flags.GetFlags(includeZero).ForEach(f => Assert.That(f.OneFlag(includeZero))).Count() == count);
	}

	[TestCase(TestFlags.A, TestFlags.L)]
	[TestCase(TestFlags.B, TestFlags.C, TestFlags.J)]
	public void When_SetFlags_Then_SameFlags(params TestFlags[] flags)
	{
		Assert.That(flags.SetFlags() == flags.Aggregate((f1, f2) => f1 | f2));
	}

	[TestCase(TestFlags.C | TestFlags.K, TestFlags.L | TestFlags.N, true)]
	[TestCase(TestFlags.A | TestFlags.E, TestFlags.O, true)]
	[TestCase(TestFlags.D, TestFlags.O | TestFlags.G, false)]
	[TestCase(TestFlags.F, TestFlags.I, false)]
	public void When_SetFlags_Then_AnyFlags(TestFlags flags1, TestFlags flags2, bool expected)
	{
		Assert.That(flags1.AnyFlags(flags2) == expected);
	}
}