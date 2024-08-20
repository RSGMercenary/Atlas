using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.SystemManager;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;
using System;

namespace Atlas.Tests.ECS.Components;

[TestFixture]
class SystemManagerTests
{
	#region Add
	[TestCase<TestSystem>]
	[TestCase<TestFamilySystem>]
	[TestCase<TestMultipleSystem1>]
	[TestCase<TestMultipleSystem2>]
	public void When_AddSystem_Then_SystemAdded<T>()
		where T : class, ISystem, new()
	{
		var component = new AtlasSystemManager();
		component.AddSystem<T>();

		Assert.That(component.Systems.Count == 1);
		Assert.That(component.HasSystem<T>());
		Assert.That(component.Systems[0] == typeof(T));
	}

	[Test]
	public void When_AddSystem_Twice_Then_NoSystemAdded()
	{
		var component = new AtlasSystemManager();
		component.AddSystem<TestSystem>();

		Assert.That(!component.AddSystem<TestSystem>());
		Assert.That(component.HasSystem<TestSystem>());
		Assert.That(component.Systems.Count == 1);
		Assert.That(component.Systems[0] == typeof(TestSystem));
	}

	[TestCase(null)]
	[TestCase(typeof(IComponent))]
	public void When_AddSystem_Then_NoSystemAdded(Type type)
	{
		var component = new AtlasSystemManager();

		Assert.That(!component.AddSystem(type));
		Assert.That(component.Systems.Count == 0);
	}

	[Test]
	public void When_AddSystem_Then_SystemAddedToEngine()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();

		var component = new AtlasSystemManager(typeof(ITestSystem));


		root.AddComponent<ISystemManager>(component);
		root.AddComponent<IEngine>(engine);



		Assert.That(engine.HasSystem<ITestSystem>());
	}
	#endregion

	#region Remove
	[TestCase<TestSystem>]
	[TestCase<TestFamilySystem>]
	[TestCase<TestMultipleSystem1>]
	[TestCase<TestMultipleSystem2>]
	public void When_RemoveSystem_Then_SystemRemoved<T>()
		where T : class, ISystem, new()
	{
		var component = new AtlasSystemManager();
		component.AddSystem<T>();
		component.RemoveSystem<T>();

		Assert.That(component.Systems.Count == 0);
		Assert.That(!component.HasSystem<T>());
	}

	[TestCase(null)]
	[TestCase(typeof(IComponent))]
	public void When_RemoveSystem_Then_NoSystemRemoved(Type type)
	{
		var component = new AtlasSystemManager();

		Assert.That(!component.RemoveSystem(type));
		Assert.That(component.Systems.Count == 0);
	}

	[Test]
	public void When_RemoveSystems_Then_SystemsRemoved()
	{
		var component = new AtlasSystemManager();
		component.AddSystem<TestSystem>();
		component.AddSystem<TestFamilySystem>();

		Assert.That(component.RemoveSystems());
		Assert.That(component.Systems.Count == 0);
	}

	[Test]
	public void When_RemoveSystems_Then_NoSystemsRemoved()
	{
		var component = new AtlasSystemManager();

		Assert.That(!component.RemoveSystems());
		Assert.That(component.Systems.Count == 0);
	}

	[Test]
	public void When_RemoveSystem_Then_SystemRemovedFromEngine()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();
		var component = new AtlasSystemManager(typeof(ITestSystem));

		root.AddComponent<ISystemManager>(component);
		root.AddComponent<IEngine>(engine);

		root.RemoveComponent<ISystemManager>();

		Assert.That(!engine.HasSystem<ITestSystem>());
	}

	[Test]
	public void When_RemoveSystem_Then_SystemRemovedFromEnginez()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();
		var component = new AtlasSystemManager();

		//Component
		component.AddSystem<ITestSystem>();
		Assert.That(component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 1);
		component.RemoveSystem<ITestSystem>();
		Assert.That(!component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 0);

		//Component & Entity
		root.AddComponent<ISystemManager>(component);

		component.AddSystem<ITestSystem>();
		Assert.That(component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 1);
		component.RemoveSystem<ITestSystem>();
		Assert.That(!component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 0);

		//Component & Entity & Engine
		root.AddComponent<IEngine>(engine);

		component.AddSystem<ITestSystem>();
		Assert.That(component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 1);
		Assert.That(engine.HasSystem<ITestSystem>());
		Assert.That(engine.Systems.Count == 1);
		component.RemoveSystem<ITestSystem>();
		Assert.That(!component.HasSystem<ITestSystem>());
		Assert.That(component.Systems.Count == 0);
		Assert.That(!engine.HasSystem<ITestSystem>());
		Assert.That(engine.Systems.Count == 0);
	}
	#endregion
}