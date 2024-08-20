using Atlas.Core.Extensions;
using NUnit.Framework;
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
}