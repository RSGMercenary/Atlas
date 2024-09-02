using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.Tests.Attributes;
using Atlas.Tests.Testers.Components;
using Atlas.Tests.Testers.Families;
using Atlas.Tests.Testers.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components.Engine;

[TestFixture]
internal class FamilyManagerTests
{
	public AtlasEngine Engine;

	[SetUp]
	public void SetUp()
	{
		Engine = new AtlasEngine();
	}

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
}