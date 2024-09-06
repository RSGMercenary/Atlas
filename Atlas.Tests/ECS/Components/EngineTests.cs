using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.Tests.Testers.Components;
using Atlas.Tests.Testers.Families;
using Atlas.Tests.Testers.Systems;
using Atlas.Tests.Testers.Utilities;
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

	#region Updates
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
		if(entitySleeping)
			root.Sleep(entitySleeping);

		var system = engine.Systems.Add<TestVariableFamilySystem>();
		system.IgnoreSleep = systemSleeping;

		engine.Updates.Update(0.125f);

		Assert.That(component.TestUpdate == expected);
	}
	#endregion

	#region Engine Manager
	[Test]
	public void When_Engine_And_HasNoEngineItem_Then_NoEngine()
	{
		var item = new EngineManager<IEntity>(null);
		item.Engine = new AtlasEngine();

		Assert.That(item.Engine == null);
	}
	#endregion

	#region Entities
	[Test]
	public void When_AddEntity_DuringUpdate_Then_EntityAdded()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestVariableFamilySystem>();

		system.TestAddEntity = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 2);
	}

	[Test]
	public void When_RemoveEntity_DuringUpdate_Then_EntityRemoved()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestVariableFamilySystem>();

		system.TestRemoveEntity = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>().Members.Count == 0);
	}
	#endregion

	#region Systems
	[Test]
	public void When_RemoveSystem_DuringUpdate_Then_SystemRemoved()
	{
		var root = GetRoot();
		var engine = root.GetComponent<AtlasEngine>();
		var system = engine.Systems.Get<TestVariableFamilySystem>();

		system.TestRemoveEntity = true;
		system.TestRemoveSystem = true;
		engine.Updates.Update(0.125f);

		Assert.That(engine.Families.Get<TestFamilyMember>() == null);
		Assert.That(!engine.Families.Has<TestFamilyMember>());
		Assert.That(engine.Systems.Get<TestVariableFamilySystem>() == null);
		Assert.That(!engine.Systems.Has<TestVariableFamilySystem>());
	}

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
	#endregion

	private IEntity GetRoot()
	{
		var root = new AtlasEntity(true);
		var entity = new AtlasEntity();

		var engine = new AtlasEngine();
		var component = new TestComponent();

		root.AddChild(entity);
		root.AddComponent(engine);
		entity.AddComponent(component);

		engine.Systems.Add<TestVariableFamilySystem>();

		return root;
	}
}