using Atlas.Core.Collections.Pool;
using Atlas.Tests.Core.Collections.Classes;
using NUnit.Framework;
using System;

namespace Atlas.Tests.Core.Collections;

[TestFixture]
internal class PoolTests
{
	private IPool<TestInstance> Pool;
	private Random Random = new Random();

	[SetUp]
	public void SetUp()
	{
		Pool = new Pool<TestInstance>();
	}

	[Test]
	public void When_Put_Then_Pooled()
	{
		Pool.Put(new TestInstance());

		Assert.That(Pool.Count == 1);
	}

	[Test]
	public void When_Get_Then_Unpooled()
	{
		Pool.Put(new TestInstance());
		Pool.Get();

		Assert.That(Pool.Count == 0);
	}

	[Test]
	[Repeat(20)]
	public void When_Fill_Then_Filled()
	{
		var maxCount = Random.Next(101);
		var pool = new Pool<TestInstance>(null, maxCount, true);

		Assert.That(pool.Count == maxCount);
		Assert.That(pool.MaxCount == maxCount);
	}

	[Test]
	[Repeat(20)]
	public void When_Empty_Then_Emptied()
	{
		var maxCount = Random.Next(101);
		var pool = new Pool<TestInstance>(null, maxCount, true);
		pool.Empty();

		Assert.That(pool.Count == 0);
		Assert.That(pool.MaxCount == maxCount);
	}
}