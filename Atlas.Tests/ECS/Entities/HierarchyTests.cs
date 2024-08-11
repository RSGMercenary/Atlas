using Atlas.ECS.Entities;
using NUnit.Framework;
using System.Collections;

namespace Atlas.Tests.ECS.Entities;

[TestFixture]
class HierarchyTests
{
    #region Add
    [Test]
    public void When_AddChild_Then_ChildAdded()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        parent.AddChild(child);

        Assert.That(parent.HasChild(child));
        Assert.That(child.Parent == parent);
    }

    [TestCase(2, 0)]
    [TestCase(5, 1)]
    [TestCase(7, 3)]
    [TestCase(10, 7)]
    public void When_SetIndex_Then_IndexSet(int count, int index)
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        for (var i = 0; i < count; ++i)
            parent.AddChild(new AtlasEntity());

        parent[index] = child;

        Assert.That(parent.GetChild(index) == child);
        Assert.That(parent.GetChildIndex(child) == index);
        Assert.That(parent.Children.Count == count);
    }

    [Test]
    public void When_AddChild_Twice_Then_ChildAdded()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        parent.AddChild(child);
        parent.AddChild(child);

        Assert.That(parent.HasChild(child));
        Assert.That(child.Parent == parent);
    }
    #endregion

    #region Remove
    [Test]
    public void When_RemoveChild_Then_ChildRemoved()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        parent.AddChild(child);
        parent.RemoveChild(child);

        Assert.That(!parent.HasChild(child));
        Assert.That(child.Parent == null);
        Assert.That(child.ParentIndex == -1);
    }

    [Test]
    public void When_RemoveChild_Then_NoChildRemoved()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        var entity = parent.RemoveChild(child);

        Assert.That(entity == null);
        Assert.That(!parent.HasChild(child));
        Assert.That(child.Parent == null);
    }

    [TestCase(1, 0)]
    [TestCase(2, 1)]
    [TestCase(7, 4)]
    [TestCase(11, 7)]
    public void When_RemoveChild_AsIndex_Then_ChildRemoved(int count, int index)
    {
        var parent = new AtlasEntity();

        for (int i = 0; i < count; i++)
            parent.AddChild(new AtlasEntity());

        var child = parent[index];
        parent.RemoveChild(index);

        Assert.That(parent.Children.Count == count - 1);
        Assert.That(!parent.HasChild(child));
        Assert.That(child.Parent == null);
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(2)]
    [TestCase(7)]
    [TestCase(11)]
    public void When_RemoveChildren_Then_ChildrenRemoved(int count)
    {
        var parent = new AtlasEntity();
        var children = Enumerable.Range(0, count).Select(i => new AtlasEntity()).ToList();

        foreach (var child in children)
            parent.AddChild(child);

        parent.RemoveChildren();

        foreach (var child in children)
        {
            Assert.That(child.Parent == null);
            Assert.That(child.ParentIndex == -1);
        }


        Assert.That(parent.Children.Count == 0);
    }
    #endregion

    #region Swap
    [TestCase(2, 0, 1)]
    [TestCase(5, 0, 4)]
    [TestCase(7, 3, 4)]
    [TestCase(10, 2, 7)]
    public void When_SwapChildren_As_Entity_Then_ChildrenSwapped(int count, int index1, int index2)
    {
        var parent = new AtlasEntity();

        for (var i = 0; i < count; ++i)
            parent.AddChild(new AtlasEntity());

        var child1 = parent[index1];
        var child2 = parent[index2];

        parent.SwapChildren(child1, child2);

        Assert.That(parent[index1] == child2);
        Assert.That(parent[index2] == child1);
    }

    [TestCase(true, true)]
    [TestCase(false, true)]
    [TestCase(true, false)]
    public void When_SwapChildren_As_BadEntity_Then_NoChildrenSwapped(bool null1, bool null2)
    {
        var parent = new AtlasEntity();
        var child1 = new AtlasEntity();
        var child2 = new AtlasEntity();

        parent.AddChild(child1);
        parent.AddChild(child2);

        child1 = null1 ? null : child1;
        child2 = null2 ? null : child2;

        Assert.That(!parent.SwapChildren(child1, child2));
    }

    [TestCase(2, 0, 1)]
    [TestCase(5, 0, 4)]
    [TestCase(7, 3, 4)]
    [TestCase(10, 2, 7)]
    public void When_SwapChildren_As_Index_Then_ChildrenSwapped(int count, int index1, int index2)
    {
        var parent = new AtlasEntity();

        for (var i = 0; i < count; ++i)
            parent.AddChild(new AtlasEntity());

        var child1 = parent[index1];
        var child2 = parent[index2];

        parent.SwapChildren(index1, index2);

        Assert.That(parent[index1] == child2);
        Assert.That(parent[index2] == child1);
    }

    [TestCase(-1, 0)]
    [TestCase(0, 2)]
    [TestCase(-1, 2)]
    public void When_SwapChildren_As_BadIndex_Then_NoChildrenSwapped(int index1, int index2)
    {
        var parent = new AtlasEntity();
        var child1 = new AtlasEntity();
        var child2 = new AtlasEntity();

        parent.AddChild(child1);
        parent.AddChild(child2);

        Assert.That(!parent.SwapChildren(index1, index2));
    }
    #endregion

    #region Parent
    [Test]
    public void When_Parent_IsNotNull_Then_HasParent()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        child.Parent = parent;

        Assert.That(parent.HasChild(child));
        Assert.That(child.Parent == parent);
    }

    [Test]
    public void When_Parent_IsNull_Then_HasNoParent()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();

        child.Parent = parent;
        child.Parent = null;

        Assert.That(!parent.HasChild(child));
        Assert.That(child.Parent == null);
    }

    [TestCase(2, 0, 1)]
    [TestCase(5, 0, 4)]
    [TestCase(7, 3, 4)]
    [TestCase(10, 2, 7)]
    public void When_ParentIndex_Then_IndexExpected(int count, int index1, int index2)
    {
        var parent = new AtlasEntity();

        for (var i = 0; i < count; i++)
            parent.AddChild(new AtlasEntity());

        var child = parent[index1];

        child.ParentIndex = index2;

        Assert.That(parent[index2] == child);
        Assert.That(child.ParentIndex == index2);
    }

    [Test]
    public void When_ParentIndex_Then_IndexIs()
    {
        var child = new AtlasEntity();

        child.ParentIndex = 5;

        Assert.That(child.ParentIndex == -1);
    }
    #endregion

    #region Root
    [TestCase(true)]
    [TestCase(false)]
    public void When_IsRoot_Then_RootExpected(bool isRoot)
    {
        var entity = new AtlasEntity(isRoot);

        Assert.That(entity.IsRoot == isRoot);
        Assert.That(entity.Root == (isRoot ? entity : null));
    }

    [Test]
    public void When_IsRoot_Reset_Then_RootFalse()
    {
        var entity = new AtlasEntity(true);

        entity.IsRoot = false;

        Assert.That(entity.IsRoot == false);
        Assert.That(entity.Root == null);
    }

    [Test]
    public void When_GetEnumerator_Then_Enumerate()
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity();
        var enumerator = (parent as IEnumerable).GetEnumerator();

        parent.AddChild(child);

        Assert.That(enumerator.MoveNext());
        Assert.That(enumerator.Current == child);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void When_SetParent_With_IsRoot_Then_ThrowsExpected(bool isRoot)
    {
        var parent = new AtlasEntity();
        var child = new AtlasEntity(isRoot);

        Assert.That(() => child.SetParent(parent), isRoot ? Throws.Exception : Throws.Nothing);
    }
    #endregion

    #region Siblings
    [TestCase(true, true, true)]
    [TestCase(false, false, false)]
    [TestCase(true, false, false)]
    [TestCase(false, true, false)]
    public void When_HasSibling_Then_SiblingExpected(bool addChild1, bool addChild2, bool expected)
    {
        var parent = new AtlasEntity();
        var child1 = new AtlasEntity();
        var child2 = new AtlasEntity();

        if (addChild1)
            parent.AddChild(child1);
        if (addChild2)
            parent.AddChild(child2);

        Assert.That(child1.HasSibling(child2) == expected);
        Assert.That(child2.HasSibling(child1) == expected);
    }

    [Test]
    public void When_HasSibling_With_BadSibling_Then_HasSiblingFalse()
    {
        var parent = new AtlasEntity();
        var child1 = new AtlasEntity();
        var child2 = new AtlasEntity();

        parent.AddChild(child1);
        parent.AddChild(child2);

        Assert.That(parent.HasSibling(new AtlasEntity()) == false);
    }
    #endregion

    #region Ancestors / Descendants
    [TestCase(1)]
    [TestCase(3)]
    [TestCase(7)]
    [TestCase(12)]
    public void When_HasDescendant_And_HasAncestor_Then_ReturnTrue(int depth)
    {
        IEntity ancestor = new AtlasEntity();
        IEntity descendant = ancestor;

        for (int i = 0; i < depth; i++)
            descendant = descendant.AddChild(new AtlasEntity());

        Assert.That(ancestor.HasDescendant(descendant));
        Assert.That(descendant.HasAncestor(ancestor));
    }

    [Test]
    public void When_HasDescendant_With_NoDescendants_Then_ReturnFalse()
    {
        var entity = new AtlasEntity();

        Assert.That(!entity.HasDescendant(entity));
        Assert.That(!entity.HasDescendant(null));
        Assert.That(!entity.HasDescendant(new AtlasEntity()));
    }

    [Test]
    public void When_HasAncestor_With_NoAncestors_Then_ReturnFalse()
    {
        var entity = new AtlasEntity();

        Assert.That(!entity.HasAncestor(entity));
        Assert.That(!entity.HasAncestor(null));
        Assert.That(!entity.HasAncestor(new AtlasEntity()));
    }
    #endregion
}