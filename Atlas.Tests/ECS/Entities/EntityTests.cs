using Atlas.ECS.Entities;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Entities;

[TestFixture]
class EntityTests
{
	#region Names
	[TestCase(true)]
	[TestCase(false)]
	public void When_IsRoot_Then_PropertiesExpected(bool isRoot)
	{
		var entity = new AtlasEntity(isRoot);

		Assert.That(entity.GlobalName == AtlasEntity.RootName == isRoot);
		Assert.That(entity.LocalName == AtlasEntity.RootName == isRoot);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_IsRoot_Reset_Then_PropertiesExpected(bool isRoot)
	{
		var entity = new AtlasEntity(!isRoot);

		entity.IsRoot = isRoot;

		Assert.That(entity.GlobalName == AtlasEntity.RootName == isRoot);
		Assert.That(entity.LocalName == AtlasEntity.RootName == isRoot);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_IsRoot_And_NamesInvalid_Then_ThrowsException(bool isRoot)
	{
		var entity = new AtlasEntity();
		entity.IsRoot = isRoot;
		var name = isRoot ? AtlasEntity.UniqueName : AtlasEntity.RootName;

		Assert.That(() => entity.GlobalName = name, Throws.Exception);
		Assert.That(() => entity.LocalName = name, Throws.Exception);
	}

	[TestCase("GlobalName")]
	[TestCase("GlobalNameButLonger")]
	public void When_GlobalName_Then_LocalNameExpected(string globalName)
	{
		var entity = new AtlasEntity("GlobalName");

		entity.GlobalName = globalName;

		Assert.That(entity.GlobalName == globalName);
	}

	[TestCase("LocalName")]
	[TestCase("LocalNameButLonger")]
	public void When_LocalName_Then_LocalNameExpected(string localName)
	{
		var entity = new AtlasEntity(null, "LocalName");

		entity.LocalName = localName;

		Assert.That(entity.LocalName == localName);
	}

	[Test]
	public void When_LocalName_With_SameLocalName_Then_ThrowsException()
	{
		const string localName = "Child";

		var parent = new AtlasEntity();
		var child1 = new AtlasEntity();
		var child2 = new AtlasEntity();

		parent.AddChild(child1);
		parent.AddChild(child2);

		child1.LocalName = localName;

		Assert.That(() => child2.LocalName = localName, Throws.Exception);
	}

	[Test]
	public void When_AddChild_With_SameLocalName_Then_ThrowsException()
	{
		const string localName = "Child";

		var parent = new AtlasEntity();
		var child1 = new AtlasEntity(null, localName);
		var child2 = new AtlasEntity(null, localName);

		parent.AddChild(child1);

		Assert.That(() => parent.AddChild(child2), Throws.Exception);
	}
	#endregion

	#region Sleeping
	[TestCase(true)]
	[TestCase(false)]
	public void When_IsSleeping_Then_IsSleepingExpected(bool isSleeping)
	{
		var entity = new AtlasEntity();

		entity.Sleep(isSleeping);

		Assert.That(entity.Sleeping == (isSleeping ? 1 : -1));
		Assert.That(entity.IsSleeping == isSleeping);
	}

	[TestCase(true)]
	[TestCase(false)]
	public void When_IsSelfSleeping_Then_IsSelfSleepingExpected(bool isSelfSleeping)
	{
		var entity = new AtlasEntity();

		entity.SelfSleep(isSelfSleeping);

		Assert.That(entity.SelfSleeping == (isSelfSleeping ? 1 : -1));
		Assert.That(entity.IsSelfSleeping == isSelfSleeping);
	}

	[TestCase(true, false, true)]
	[TestCase(true, true, false)]
	[TestCase(false, false, false)]
	[TestCase(false, true, false)]
	public void When_ParentIsSleeping_And_ChildIsSelfSleeping_Then_ChildIsSleepingExpected(bool parentIsSleeping, bool childIsSelfSleeping, bool expected)
	{
		var parent = new AtlasEntity();
		var child = new AtlasEntity();

		parent.AddChild(child);

		parent.Sleep(parentIsSleeping);
		child.SelfSleep(childIsSelfSleeping);

		Assert.That(parent.IsSleeping == parentIsSleeping);
		Assert.That(child.IsSelfSleeping == childIsSelfSleeping);
		Assert.That(child.IsSleeping == expected);
	}

	[Test]
	public void When_ParentIsSleeping_And_ChildIsSelfSleeping_Changed_Then_ChildIsSleepingExpected()
	{
		var parent = new AtlasEntity();
		var child = new AtlasEntity();

		parent.AddChild(child);

		parent.Sleep(true);
		child.SelfSleep(true);
		child.SelfSleep(false);

		Assert.That(parent.IsSleeping == true);
		Assert.That(child.IsSleeping == true);
	}
	#endregion

	#region Dispose
	[TestCase(true)]
	[TestCase(false)]
	public void When_IsAutoDisposable_Then_IsAutoDisposableExpected(bool isAutoDisposable)
	{
		var entity = new AtlasEntity();

		entity.AutoDispose = isAutoDisposable;

		Assert.That(entity.AutoDispose == isAutoDisposable);
	}

	[Test]
	public void When_Dispose_Then_Disposed()
	{
		var entity = new AtlasEntity();

		entity.AddChild(new AtlasEntity());

		entity.Dispose();

		Assert.That(entity.Children.Count == 0);
	}
	#endregion
}