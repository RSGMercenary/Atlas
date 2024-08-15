using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Families.Families;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components;

[TestFixture]
class EngineTests
{
	public AtlasEngine Engine;

	[SetUp]
	public void SetUp()
	{
		Engine = new AtlasEngine();
	}

	#region Entities
	#region Add
	[Test]
	public void When_AddEngine_Then_HasRoot()
	{
		var root = new AtlasEntity(true);

		root.AddComponent<IEngine>(Engine);

		Assert.That(Engine.HasEntity(root));
		Assert.That(Engine.HasEntity(root.GlobalName));
		Assert.That(Engine.GetEntity(root.GlobalName) == root);
		Assert.That(Engine.Entities.Count == 1);
	}

	[Test]
	public void When_AddEngine_With_Children_Then_HasChildren()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddChild(child);
		root.AddComponent<IEngine>(Engine);

		Assert.That(Engine.HasEntity(child));
		Assert.That(Engine.HasEntity(child.GlobalName));
		Assert.That(Engine.GetEntity(child.GlobalName) == child);
		Assert.That(Engine.Entities.Count == 2);
	}

	[Test]
	public void When_AddChild_Then_EntityAdded()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddComponent<IEngine>(Engine);
		root.AddChild(child);

		Assert.That(Engine.HasEntity(child));
		Assert.That(Engine.HasEntity(child.GlobalName));
		Assert.That(Engine.GetEntity(child.GlobalName) == child);
		Assert.That(Engine.Entities.Count == 2);
	}
	#endregion

	#region Remove
	[Test]
	public void When_EngineRemoveFromRoot_Then_HasNoRoot()
	{
		var root = new AtlasEntity(true);

		root.AddComponent<IEngine>(Engine);
		root.RemoveComponent<IEngine>();

		Assert.That(!Engine.HasEntity(root));
		Assert.That(!Engine.HasEntity(root.GlobalName));
		Assert.That(Engine.GetEntity(root.GlobalName) == null);
		Assert.That(Engine.Entities.Count == 0);
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

		Assert.That(!Engine.HasEntity(child));
		Assert.That(!Engine.HasEntity(child.GlobalName));
		Assert.That(Engine.GetEntity(child.GlobalName) == null);
		Assert.That(Engine.Entities.Count == 0);
	}

	[Test]
	public void When_RemoveChild_Then_EntityRemoved()
	{
		var root = new AtlasEntity(true);
		var child = new AtlasEntity();

		root.AddComponent<IEngine>(Engine);
		root.AddChild(child);
		root.RemoveChild(child);

		Assert.That(!Engine.HasEntity(child));
		Assert.That(!Engine.HasEntity(child.GlobalName));
		Assert.That(Engine.GetEntity(child.GlobalName) == null);
		Assert.That(Engine.Entities.Count == 1);
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
		Assert.That(Engine.HasEntity(child1));
		Assert.That(Engine.HasEntity(child2));
		Assert.That(Engine.HasEntity(child1Name));
		Assert.That(Engine.HasEntity(child2Name));
		Assert.That(Engine.GetEntity(child1Name) == child1);
		Assert.That(Engine.GetEntity(child2Name) == child2);
		Assert.That(Engine.Entities.Count == 3);
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

		Assert.That(!Engine.HasEntity(oldName));
		Assert.That(Engine.HasEntity(newName));
		Assert.That(Engine.HasEntity(child));
		Assert.That(Engine.GetEntity(newName) == child);
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

		Assert.That(Engine.HasEntity(child1.GlobalName));
		Assert.That(Engine.HasEntity(child2.GlobalName));
		Assert.That(Engine.HasEntity(child1));
		Assert.That(Engine.HasEntity(child2));
		Assert.That(Engine.GetEntity(child1.GlobalName) == child1);
		Assert.That(Engine.GetEntity(child2.GlobalName) == child2);
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
	#endregion

	#region Families
	#region Add
	[TestCase<TestFamilyMember>]
	public void When_AddFamily_Then_FamilyAdded<T>()
		where T : class, IFamilyMember, new()
	{
		var family = Engine.AddFamily<T>();

		Assert.That(Engine.HasFamily<T>());
		Assert.That(Engine.GetFamily<T>() == family);
		Assert.That(Engine.Families.Count == 1);
	}
	#endregion

	#region Remove
	[TestCase<TestFamilyMember>]
	public void When_RemoveFamily_Then_FamilyRemoved<T>()
		where T : class, IFamilyMember, new()
	{
		var family = Engine.AddFamily<T>();
		Engine.RemoveFamily<T>();

		Assert.That(!Engine.HasFamily<T>());
		Assert.That(Engine.GetFamily<T>() == null);
		Assert.That(Engine.Families.Count == 0);
	}
	#endregion
	#endregion

	#region Systems
	#region Add
	[TestCase<ITestSystem>]
	[TestCase<TestSystem>]
	public void When_AddSystem_Then_SystemAdded<T>()
		where T : class, ISystem
	{
		var system = Engine.AddSystem<T>();

		Assert.That(Engine.HasSystem<T>());
		Assert.That(Engine.HasSystem(system));
		Assert.That(Engine.GetSystem<T>() == system);
		Assert.That(Engine.Systems.Count == 1);
	}

	[TestCase(typeof(ITestSystem))]
	[TestCase(typeof(TestSystem))]
	public void When_AddSystem_Then_SystemAdded(Type type)
	{
		var system = Engine.AddSystem(type);

		Assert.That(Engine.HasSystem(type));
		Assert.That(Engine.HasSystem(system));
		Assert.That(Engine.GetSystem(type) == system);
		Assert.That(Engine.Systems.Count == 1);
	}

	[TestCase<ITestSystem>]
	public void When_AddSystem_Twice_Then_SystemAdded<T>()
		where T : class, ISystem
	{
		var system = Engine.AddSystem<T>();
		Engine.AddSystem<T>();

		Assert.That(Engine.HasSystem<T>());
		Assert.That(Engine.HasSystem(system));
		Assert.That(Engine.GetSystem<T>() == system);
		Assert.That(Engine.Systems.Count == 1);
	}
	#endregion

	#region Remove
	[TestCase<ITestSystem>]
	[TestCase<TestSystem>]
	public void When_RemoveSystem_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		var engine = new AtlasEngine();

		var system = engine.AddSystem<T>();
		engine.RemoveSystem<T>();

		Assert.That(!engine.HasSystem<T>());
		Assert.That(!Engine.HasSystem(system));
		Assert.That(engine.GetSystem<T>() == null);
		Assert.That(engine.Systems.Count == 0);
	}

	[TestCase(typeof(ITestSystem))]
	[TestCase(typeof(TestSystem))]
	public void When_RemoveSystem_Then_SystemRemoved(Type type)
	{
		var engine = new AtlasEngine();

		var system = engine.AddSystem(type);
		engine.RemoveSystem(type);

		Assert.That(!engine.HasSystem(type));
		Assert.That(!Engine.HasSystem(system));
		Assert.That(engine.GetSystem(type) == null);
		Assert.That(engine.Systems.Count == 0);
	}

	[TestCase<ITestSystem>]
	public void When_RemoveSystem_Twice_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		var system = Engine.AddSystem<T>();
		Engine.AddSystem<T>();
		Engine.RemoveSystem<T>();
		Engine.RemoveSystem<T>();

		Assert.That(!Engine.HasSystem<T>());
		Assert.That(!Engine.HasSystem(system));
		Assert.That(Engine.GetSystem<T>() == null);
		Assert.That(Engine.Systems.Count == 0);
	}

	[TestCase<ITestSystem>]
	public void When_RemoveSystem_With_NoSystem_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		Engine.RemoveSystem<T>();

		Assert.That(!Engine.HasSystem<T>());
		Assert.That(Engine.GetSystem<T>() == null);
		Assert.That(Engine.Systems.Count == 0);
	}
	#endregion

	#region Get
	[TestCase(true)]
	[TestCase(false)]
	public void When_GetSystem_As_Index_Then_ThrowsExpected(bool addSystem)
	{
		if(addSystem)
			Engine.AddSystem<ITestSystem>();

		Assert.That(() => Engine.GetSystem(0), addSystem ? Throws.Nothing : Throws.Exception);
	}
	#endregion

	#region Priority
	[Test]
	public void When_PriorityChanged_Then_SystemsPrioritized()
	{
		var system1 = Engine.AddSystem<TestMultipleSystem1>();
		var system2 = Engine.AddSystem<TestMultipleSystem2>();

		system2.Priority = -1;

		Assert.That(Engine.Systems[0] == system2);
		Assert.That(Engine.Systems[1] == system1);
	}
	#endregion
	#endregion

	#region Updates
	#region Updates
	[TestCase(TestFps._0, false)]
	[TestCase(TestFps._30, true)]
	[TestCase(TestFps._60, true)]
	[TestCase(TestFps._120, true)]
	[TestCase(TestFps._1, true)]
	public void When_Update_Then_SystemUpdated(float deltaTime, bool expected)
	{
		var updated = false;

		var system = Engine.AddSystem<TestSystem>();
		system.AddListener<IUpdateStateMessage<ISystem>>(_ => updated = system.Engine != null);

		Engine.Update(deltaTime);

		Assert.That(updated == expected);
	}

	[TestCase(30, 0)]
	[TestCase(16, 1)]
	[TestCase(13, 2)]
	[TestCase(11, 3)]
	[TestCase(10, 4)]
	public void When_Update_Then_FixedLagExpected(float fps, int expected)
	{
		Engine.Update(1 / fps);
		Engine.Update(TestFps._60);

		Assert.That(Engine.FixedLag == expected);
	}

	[TestCase(TestFps._0, TestFps._0)]
	[TestCase(TestFps._30, TestFps._60)]
	[TestCase(TestFps._60, TestFps._120)]
	[TestCase(TestFps._120, TestFps._120)]
	[TestCase(TestFps._120, TestFps._60)]
	[TestCase(TestFps._60, TestFps._30)]
	[TestCase(0.25f, TestFps._1)]
	public void When_Update_Then_DeltaVariableTimeExpected(float maxVariableTime, float deltaVariableTime)
	{
		Engine.MaxVariableTime = maxVariableTime;

		Engine.Update(deltaVariableTime);

		Assert.That(Engine.DeltaVariableTime == float.MinNumber(maxVariableTime, deltaVariableTime));
	}

	[TestCase(TestFps._0, TestFps._0, 0f)]
	[TestCase(TestFps._60, TestFps._120, 0.5f)]
	[TestCase(TestFps._30, TestFps._120, 0.25f)]
	[TestCase(TestFps._30, TestFps._60, 0.5f)]
	[TestCase(TestFps._15, TestFps._60, 0.25f)]
	[TestCase(TestFps._15, TestFps._120, 0.125f)]
	public void When_Update_Then_VariableInterpolationExpected(float deltaFixedTime, float deltaTime, float variableInterpolation)
	{
		Engine.DeltaFixedTime = deltaFixedTime;
		Engine.MaxVariableTime = deltaTime;

		Engine.Update(deltaTime);

		Assert.That(Engine.VariableInterpolation == variableInterpolation);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_MaxVariableTime_Then_MaxVariableTimeExpected(float maxVariableTime)
	{
		Engine.MaxVariableTime = maxVariableTime;

		Assert.That(Engine.MaxVariableTime == maxVariableTime);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_DeltaFixedTime_Then_DeltaFixedTimeExpected(float deltaFixedTime)
	{
		Engine.DeltaFixedTime = deltaFixedTime;
		Engine.Update(deltaFixedTime);

		Assert.That(Engine.DeltaFixedTime == deltaFixedTime);
	}
	#endregion
	#endregion
}