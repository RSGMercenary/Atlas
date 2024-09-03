using Atlas.Core.Extensions;
using Atlas.Tests.Testers.Utilities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.Tests.Core.Extensions;

[TestFixture]
class CollectionExtensionsTests
{
	[TestCase(100)]
	[TestCase(123.456f)]
	[TestCase(987.654321d)]
	[TestCase('X')]
	[TestCase("Testing")]
	public void When_ToEnumerable_Then_ValuesExpected<T>(T item)
	{
		var items = item.ToEnumerable();

		Assert.That(items.Contains(item));
		Assert.That(items.Count() == 1);
		Assert.That(items.Single().Equals(item));
	}

	[TestCase(300f, 100f, 100f, 100f)]
	[TestCase(4f, 145.45f, 0.4f, 2846.39f)]
	public void When_ForEach_Then_ValuesExpected(params float[] items)
	{
		var value = 0f;
		var values = items.ForEach(i => value += i);

		Assert.That(values.Count() == items.Length);
		Assert.That(value == items.Sum());
	}

	[TestCase(300f, 101f, 102f, 103f)]
	[TestCase(4f, 145.45f, 0.4f, 2846.39f)]
	public void When_Pop_Then_ValuesExpected(params float[] items)
	{
		var values = new List<float>(items);
		var value = values.Pop();

		Assert.That(values.Count() == items.Length - 1);
		Assert.That(value == items.Last());
		Assert.That(!values.Contains(value));
		Assert.That(values.Last() != value);
	}

	[Test]
	[Repeat(20)]
	public void When_Swap_Then_Swapped()
	{
		var random = new Random();
		var count = random.Next(2, 21);
		var index1 = random.Next(0, count);
		var index2 = random.NextExclude(count, index1);
		var list = new List<int>();

		for(var i = 0; i < count; ++i)
			list.Add(random.Next(0, 21));

		var value1 = list[index1];
		var value2 = list[index2];

		list.Swap(index1, index2);

		Assert.That(value1 == list[index2]);
		Assert.That(value2 == list[index1]);
	}
}