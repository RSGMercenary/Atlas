using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.Testers.Components;
using Atlas.Tests.Testers.Families;
using Atlas.Tests.Testers.Systems;
using Atlas.Tests.Testers.Utilities;
using NUnit.Framework;
using System;

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
	#endregion

	#region Families
	#region Add
	[TestCase<TestFamilyMember>]
	public void When_AddFamily_Then_FamilyAdded<T>()
		where T : class, IFamilyMember, new()
	{
		var family = Engine.Families.Add<T>();

		Assert.That(Engine.Families.Has<T>());
		Assert.That(Engine.Families.Get<T>() == family);
		Assert.That(Engine.Families.Families.Count == 1);
	}

	[Test]
	public void When_AddEntity_Then_EntityAdded()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);
		root.AddChild(entity);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		entity.AddComponent<TestComponent>();

		Assert.That(engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Families.Get<TestFamilyMember>().GetMember(entity) != null);
		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 1);
	}

	[Test]
	public void When_AddEntity_Then_FamilyHasEntity()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		entity.AddComponent<TestComponent>();

		root.AddChild(entity);

		Assert.That(engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Families.Get<TestFamilyMember>().GetMember(entity) != null);
		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 1);
	}

	[Test]
	public void When_AddSystems_Then_FamiliesAdded()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		for(var i = 0; i < 10; ++i)
		{
			var entity = root.AddChild(new AtlasEntity());
			entity.AddComponent<TestComponent>();
		}

		Assert.That(engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 10);
		Assert.That(engine.Systems.Has<TestFamilySystem1>());
		Assert.That(engine.Systems.Has<TestFamilySystem2>());
	}
	#endregion

	#region Remove
	[TestCase<TestFamilyMember>]
	public void When_RemoveFamily_Then_FamilyRemoved<T>()
		where T : class, IFamilyMember, new()
	{
		var family = Engine.Families.Add<T>();
		Engine.Families.Remove<T>();

		Assert.That(!Engine.Families.Has<T>());
		Assert.That(Engine.Families.Get<T>() == null);
		Assert.That(Engine.Families.Families.Count == 0);
	}

	[Test]
	public void When_RemoveEntity_Then_EntityRemoved()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);
		root.AddChild(entity);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		entity.AddComponent<TestComponent>();
		entity.RemoveComponent<TestComponent>();

		Assert.That(engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Families.Get<TestFamilyMember>().GetMember(entity) == null);
		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 0);
	}

	[Test]
	public void When_RemoveEntity_Then_NoFamilyHasEntity()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		entity.AddComponent<TestComponent>();

		root.AddChild(entity);
		root.RemoveChild(entity);

		Assert.That(engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Families.Get<TestFamilyMember>().GetMember(entity) == null);
		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 0);
	}

	[TestCase<TestFamilyMember>]
	public void When_RemoveFamily_Then_NoFamilyRemoved<T>()
		where T : class, IFamilyMember, new()
	{
		Engine.Families.Remove<T>();

		Assert.That(!Engine.Families.Has<T>());
		Assert.That(Engine.Families.Get<T>() == null);
		Assert.That(Engine.Families.Families.Count == 0);
	}

	[Test]
	public void When_RemoveSystems_Then_FamiliesRemoved()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();

		root.AddComponent<IEngine>(engine);

		engine.Systems.Add<TestFamilySystem1>();
		engine.Systems.Add<TestFamilySystem2>();

		for(var i = 0; i < 10; ++i)
		{
			var entity = root.AddChild(new AtlasEntity());
			entity.AddComponent<TestComponent>();
		}

		engine.Systems.Remove<TestFamilySystem1>();
		engine.Systems.Remove<TestFamilySystem2>();

		Assert.That(!engine.Families.Has<TestFamilyMember>());
		Assert.That(!engine.Systems.Has<TestFamilySystem1>());
		Assert.That(!engine.Systems.Has<TestFamilySystem2>());
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
		var system = Engine.Systems.Add<T>();

		Assert.That(Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get<T>() == system);
		Assert.That(Engine.Systems.Systems.Count == 1);
	}

	[TestCase(typeof(ITestSystem))]
	[TestCase(typeof(TestSystem))]
	public void When_AddSystem_Then_SystemAdded(Type type)
	{
		var system = Engine.Systems.Add(type);

		Assert.That(Engine.Systems.Has(type));
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get(type) == system);
		Assert.That(Engine.Systems.Systems.Count == 1);
	}

	[TestCase<ITestSystem>]
	public void When_AddSystem_Twice_Then_SystemAdded<T>()
		where T : class, ISystem
	{
		var system = Engine.Systems.Add<T>();
		Engine.Systems.Add<T>();

		Assert.That(Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get<T>() == system);
		Assert.That(Engine.Systems.Systems.Count == 1);
	}
	#endregion

	#region Remove
	[TestCase<ITestSystem>]
	[TestCase<TestSystem>]
	public void When_RemoveSystem_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		var engine = new AtlasEngine();

		var system = engine.Systems.Add<T>();
		engine.Systems.Remove<T>();

		Assert.That(!engine.Systems.Has<T>());
		Assert.That(!Engine.Systems.Has(system));
		Assert.That(engine.Systems.Get<T>() == null);
		Assert.That(engine.Systems.Systems.Count == 0);
	}

	[TestCase(typeof(ITestSystem))]
	[TestCase(typeof(TestSystem))]
	public void When_RemoveSystem_Then_SystemRemoved(Type type)
	{
		var engine = new AtlasEngine();

		var system = engine.Systems.Add(type);
		engine.Systems.Remove(type);

		Assert.That(!engine.Systems.Has(type));
		Assert.That(!Engine.Systems.Has(system));
		Assert.That(engine.Systems.Get(type) == null);
		Assert.That(engine.Systems.Systems.Count == 0);
	}

	[TestCase<ITestSystem>]
	public void When_RemoveSystem_Twice_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		var system = Engine.Systems.Add<T>();
		Engine.Systems.Add<T>();
		Engine.Systems.Remove<T>();
		Engine.Systems.Remove<T>();

		Assert.That(!Engine.Systems.Has<T>());
		Assert.That(!Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get<T>() == null);
		Assert.That(Engine.Systems.Systems.Count == 0);
	}

	[TestCase<ITestSystem>]
	public void When_RemoveSystem_With_NoSystem_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		Engine.Systems.Remove<T>();

		Assert.That(!Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Get<T>() == null);
		Assert.That(Engine.Systems.Systems.Count == 0);
	}
	#endregion

	#region Get
	[TestCase(true)]
	[TestCase(false)]
	public void When_GetSystem_As_Index_Then_ThrowsExpected(bool addSystem)
	{
		if(addSystem)
			Engine.Systems.Add<ITestSystem>();

		Assert.That(() => Engine.Systems.Get(0), addSystem ? Throws.Nothing : Throws.Exception);
	}
	#endregion

	#region Priority
	[Test]
	public void When_PriorityChanged_Then_SystemsPrioritized()
	{
		var system1 = Engine.Systems.Add<TestMultipleSystem1>();
		var system2 = Engine.Systems.Add<TestMultipleSystem2>();

		system2.Priority = -1;

		Assert.That(Engine.Systems.Systems[0] == system2);
		Assert.That(Engine.Systems.Systems[1] == system1);
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
		var system = Engine.Systems.Add<TestSystem>();

		Engine.Updates.Update(deltaTime);

		Assert.That(system.TestUpdate == expected);
	}

	[TestCase(30, 0)]
	[TestCase(16, 1)]
	[TestCase(13, 2)]
	[TestCase(11, 3)]
	[TestCase(10, 4)]
	public void When_Update_Then_FixedLagExpected(float fps, int expected)
	{
		Engine.Updates.Update(1 / fps);
		Engine.Updates.Update(TestFps._60);

		Assert.That(Engine.Updates.FixedLag == expected);
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
		Engine.Updates.MaxVariableTime = maxVariableTime;

		Engine.Updates.Update(deltaVariableTime);

		Assert.That(Engine.Updates.DeltaVariableTime == float.MinNumber(maxVariableTime, deltaVariableTime));
	}

	[TestCase(TestFps._0, TestFps._0, 0f)]
	[TestCase(TestFps._60, TestFps._120, 0.5f)]
	[TestCase(TestFps._30, TestFps._120, 0.25f)]
	[TestCase(TestFps._30, TestFps._60, 0.5f)]
	[TestCase(TestFps._15, TestFps._60, 0.25f)]
	[TestCase(TestFps._15, TestFps._120, 0.125f)]
	public void When_Update_Then_VariableInterpolationExpected(float deltaFixedTime, float deltaTime, float variableInterpolation)
	{
		Engine.Updates.DeltaFixedTime = deltaFixedTime;
		Engine.Updates.MaxVariableTime = deltaTime;

		Engine.Updates.Update(deltaTime);

		Assert.That(Engine.Updates.VariableInterpolation == variableInterpolation);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_MaxVariableTime_Then_MaxVariableTimeExpected(float maxVariableTime)
	{
		Engine.Updates.MaxVariableTime = maxVariableTime;

		Assert.That(Engine.Updates.MaxVariableTime == maxVariableTime);
	}

	[TestCase(TestFps._0)]
	[TestCase(TestFps._30)]
	[TestCase(TestFps._60)]
	[TestCase(TestFps._120)]
	[TestCase(TestFps._1)]
	public void When_DeltaFixedTime_Then_DeltaFixedTimeExpected(float deltaFixedTime)
	{
		Engine.Updates.DeltaFixedTime = deltaFixedTime;
		Engine.Updates.Update(deltaFixedTime);

		Assert.That(Engine.Updates.DeltaFixedTime == deltaFixedTime);
	}

	[Test]
	public void When_SystemUpdate_And_NotUpdateSystem_Then_SystemNoUpdated()
	{
		var system = Engine.Systems.Add<TestSystem>();

		system.Update(0);

		Assert.That(!system.TestUpdate);
	}

	[TestCase(true, true, true)]
	[TestCase(true, false, true)]
	[TestCase(false, true, false)]
	[TestCase(false, false, true)]
	public void When_Update_And_UpdateSleepingEntities_Then_Updated(bool systemSleeping, bool entitySleeping, bool expected)
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();
		var component = new TestComponent();

		root.AddComponent(engine);
		root.AddComponent(component);
		root.IsSleeping = entitySleeping;

		var system = engine.Systems.Add<TestFamilySystem>();
		system.UpdateSleepingEntities = systemSleeping;

		engine.Updates.Update(0.125f);

		Assert.That(component.TestUpdate == expected);
	}
	#endregion
	#endregion

	#region EngineItems
	[Test]
	public void When_Engine_And_HasNoEngineItem_Then_NoEngine()
	{
		var item = new EngineManager<IEntity>(null);
		item.Engine = new AtlasEngine();

		Assert.That(item.Engine == null);
	}
	#endregion

	[Test]
	public void When_AddEntity_DuringUpdate_Then_EntityAdded()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestFamilySystem>();

		system.TestAddEntity = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 2);
	}

	[Test]
	public void When_RemoveEntity_DuringUpdate_Then_EntityRemoved()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestFamilySystem>();

		system.TestRemoveEntity = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 0);
	}

	[Test]
	public void When_RemoveSystem_DuringUpdate_Then_SystemRemoved()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestFamilySystem>();

		system.TestRemoveEntity = true;
		system.TestRemoveSystem = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>() == null);
		Assert.That(!engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Systems.Get<TestFamilySystem>() == null);
		Assert.That(!engine.Systems.Has<TestFamilySystem>());
	}

	private IEntity GetRoot()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();

		var engine = new AtlasEngine();
		var component = new TestComponent();

		root.AddChild(entity);
		root.AddComponent(engine);
		entity.AddComponent(component);

		engine.Systems.Add<TestFamilySystem>();

		return root;
	}
}