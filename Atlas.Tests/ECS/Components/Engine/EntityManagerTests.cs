using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components.Engine;

[TestFixture]
internal class EntityManagerTests
{
	public AtlasEngine Engine;

	[SetUp]
	public void SetUp()
	{
		Engine = new AtlasEngine();
	}

	#region Add
	[Test]
	public void When_AddEngine_Then_HasRoot()
	{
		var root = new AtlasEntity(true);

		root.AddComponent<IEngine>(Engine);

		Assert.That(Engine.Entities.Has(root));
		Assert.That(Engine.Entities.Has(root.GlobalName));
		Assert.That(Engine.Entities.Get(root.GlobalName) == root);
		Assert.That(Engine.Entities.Entities.Count == 1);
	}

	[Test]
	public void When_AddEngine_With_Children_Then_HasChildren()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddChild(child);
		root.AddComponent<IEngine>(Engine);

		Assert.That(Engine.Entities.Has(child));
		Assert.That(Engine.Entities.Has(child.GlobalName));
		Assert.That(Engine.Entities.Get(child.GlobalName) == child);
		Assert.That(Engine.Entities.Entities.Count == 2);
	}

	[Test]
	public void When_AddChild_Then_EntityAdded()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddComponent<IEngine>(Engine);
		root.AddChild(child);

		Assert.That(Engine.Entities.Has(child));
		Assert.That(Engine.Entities.Has(child.GlobalName));
		Assert.That(Engine.Entities.Get(child.GlobalName) == child);
		Assert.That(Engine.Entities.Entities.Count == 2);
	}
	#endregion

	#region Remove
	[Test]
	public void When_EngineRemoveFromRoot_Then_HasNoRoot()
	{
		var root = new AtlasEntity(true);

		root.AddComponent<IEngine>(Engine);
		root.RemoveComponent<IEngine>();

		Assert.That(!Engine.Entities.Has(root));
		Assert.That(!Engine.Entities.Has(root.GlobalName));
		Assert.That(Engine.Entities.Get(root.GlobalName) == null);
		Assert.That(Engine.Entities.Entities.Count == 0);
	}

	[Test]
	public void When_RemoveEngine_With_Children_Then_HasNoChildren()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		child.GlobalName = "Child";
		root.AddChild(child);

		root.AddComponent<IEngine>(Engine);
		root.RemoveComponent<IEngine>();

		Assert.That(!Engine.Entities.Has(child));
		Assert.That(!Engine.Entities.Has(child.GlobalName));
		Assert.That(Engine.Entities.Get(child.GlobalName) == null);
		Assert.That(Engine.Entities.Entities.Count == 0);
	}

	[Test]
	public void When_RemoveChild_Then_EntityRemoved()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddComponent<IEngine>(Engine);
		root.AddChild(child);
		root.RemoveChild(child);

		Assert.That(!Engine.Entities.Has(child));
		Assert.That(!Engine.Entities.Has(child.GlobalName));
		Assert.That(Engine.Entities.Get(child.GlobalName) == null);
		Assert.That(Engine.Entities.Entities.Count == 1);
	}
	#endregion

	#region Global Names
	[Test]
	public void When_AddEngine_With_SameGlobalName_Then_GlobalNameChanged()
	{
		const string child1Name = "Child";

		var root = new AtlasEntity(true);
		var child1 = new AtlasEntity();
		var child2 = new AtlasEntity();

		child1.GlobalName = child1Name;
		child2.GlobalName = child1Name;
		root.AddChild(child1);
		root.AddChild(child2);

		root.AddComponent<IEngine>(Engine);

		var child2Name = child2.GlobalName;

		Assert.That(child1.GlobalName == child1Name);
		Assert.That(child2.GlobalName != child1Name);
		Assert.That(Engine.Entities.Has(child1));
		Assert.That(Engine.Entities.Has(child2));
		Assert.That(Engine.Entities.Has(child1Name));
		Assert.That(Engine.Entities.Has(child2Name));
		Assert.That(Engine.Entities.Get(child1Name) == child1);
		Assert.That(Engine.Entities.Get(child2Name) == child2);
		Assert.That(Engine.Entities.Entities.Count == 3);
	}

	[Test]
	public void When_GlobalName_Then_HasRenamedEntity()
	{
		const string oldName = "OldName";
		const string newName = "NewName";

		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddChild(child);
		child.GlobalName = oldName;
		root.AddComponent<IEngine>(Engine);
		child.GlobalName = newName;

		Assert.That(!Engine.Entities.Has(oldName));
		Assert.That(Engine.Entities.Has(newName));
		Assert.That(Engine.Entities.Has(child));
		Assert.That(Engine.Entities.Get(newName) == child);
	}

	[Test]
	public void When_AddChild_With_SameGlobalName_Then_HasRenamedEntity()
	{
		const string name = "Child";

		var root = new AtlasEntity(true);
		var child1 = new AtlasEntity();
		var child2 = new AtlasEntity();

		child1.GlobalName = name;
		root.AddChild(child1);

		root.AddComponent<IEngine>(Engine);

		child2.GlobalName = name;
		root.AddChild(child2);

		Assert.That(Engine.Entities.Has(child1.GlobalName));
		Assert.That(Engine.Entities.Has(child2.GlobalName));
		Assert.That(Engine.Entities.Has(child1));
		Assert.That(Engine.Entities.Has(child2));
		Assert.That(Engine.Entities.Get(child1.GlobalName) == child1);
		Assert.That(Engine.Entities.Get(child2.GlobalName) == child2);
	}
	#endregion

	#region Root
	[Test]
	public void When_IsRoot_IsFalse_Then_EngineRemoved()
	{
		var root = new AtlasEntity(true);
		root.AddComponent<IEngine>(Engine);

		root.IsRoot = false;

		Assert.That(!root.HasComponent<IEngine>());
	}
	#endregion
}