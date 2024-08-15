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

    [Test]
    public void When_IsRoot_Reset_Then_PropertiesExpected()
    {
        var entity = new AtlasEntity(true);

        entity.IsRoot = false;

        Assert.That(entity.GlobalName != AtlasEntity.RootName);
        Assert.That(entity.LocalName != AtlasEntity.RootName);
    }

    [TestCase("LocalName")]
    [TestCase("LocalNameButLonger")]
    public void When_LocalName_Then_LocalNameExpected(string localName)
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity("LocalName");

        child.LocalName = localName;

        Assert.That(child.LocalName == localName);
    }

    [Test]
    public void When_NamesAreRoot_Then_ThrowsException()
    {
        var entity = new AtlasEntity();
        var name = AtlasEntity.RootName;

        Assert.That(() => entity.GlobalName = name, Throws.Exception);
        Assert.That(() => entity.LocalName = name, Throws.Exception);
    }

    [Test]
    public void When_AddChildren_With_SameLocalName_Then_LocalNameChanged()
    {
        const string localName = "Child";

        var parent = new AtlasEntity();
        var child1 = new AtlasEntity();
        var child2 = new AtlasEntity();

        parent.AddChild(child1);
        parent.AddChild(child2);

        child1.LocalName = localName;
        child2.LocalName = localName;

        Assert.That(child1.LocalName == localName);
        Assert.That(child2.LocalName != localName);
    }

    /*
	[TestCase(null, false)]
	[TestCase("NotRoot", true)]
	public void When_GlobalName_Invalid_Then_GlobalNameNotChanged(string name, bool expected)
	{
		var entity = new AtlasEntity(name, null);


		Assert.That(() => entity.GlobalName, expected);
	}*/
    #endregion

    #region Sleeping
    [TestCase(true)]
    [TestCase(false)]
    public void When_IsSleeping_Then_IsSleepingExpected(bool isSleeping)
    {
        var entity = new AtlasEntity();
        entity.IsSleeping = isSleeping;

        Assert.That(entity.IsSleeping == isSleeping);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void When_IsSelfSleeping_Then_IsSelfSleepingExpected(bool isSelfSleeping)
    {
        var entity = new AtlasEntity();
        entity.IsSelfSleeping = isSelfSleeping;

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

        parent.IsSleeping = parentIsSleeping;
        child.IsSelfSleeping = childIsSelfSleeping;

        Assert.That(parent.IsSleeping == parentIsSleeping);
        Assert.That(child.IsSelfSleeping == childIsSelfSleeping);
        Assert.That(child.IsSleeping == expected);
    }

    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    public void When_ParentIsSleeping_And_ChildIsSelfSleeping_Then_ChildIsSleepingExpectedX(bool parentIsSleeping, bool childIsSelfSleeping, bool expected)
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        parent.AddChild(child);

        parent.IsSleeping = true;
        child.IsSelfSleeping = true;
        child.IsSelfSleeping = false;

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

        entity.IsAutoDisposable = isAutoDisposable;

        Assert.That(entity.IsAutoDisposable == isAutoDisposable);
    }

    [Test]
    public void When_Dispose_Then_Disposed()
    {
        var entity = new AtlasEntity();
        var isDisposed = false;

        AtlasEntity.Disposed += _ => isDisposed = true;

        entity.Dispose();

        Assert.That(isDisposed);
    }
    #endregion
}