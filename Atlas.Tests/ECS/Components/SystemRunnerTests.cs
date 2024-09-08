using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.SystemRunner;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.Testers.Systems;
using NUnit.Framework;
using System;

namespace Atlas.Tests.ECS.Components;

[TestFixture]
class SystemRunnerTests
{
	#region Add
	[TestCase<TestSystem>]
	[TestCase<TestVariableFamilySystem>]
	[TestCase<TestMultipleSystem1>]
	[TestCase<TestMultipleSystem2>]
	public void When_AddSystem_Then_SystemAdded<T>()
		where T : class, ISystem, new()
	{
		var component = new AtlasSystemRunner();
		component.Add<T>();

		Assert.That(component.Types.Count == 1);
		Assert.That(component.Has<T>());
	}

	[Test]
	public void When_AddSystem_Twice_Then_NoSystemAdded()
	{
		var component = new AtlasSystemRunner();
		component.Add<TestSystem>();

		Assert.That(!component.Add<TestSystem>());
		Assert.That(component.Has<TestSystem>());
		Assert.That(component.Types.Count == 1);
	}

	[TestCase(null)]
	[TestCase(typeof(IComponent))]
	public void When_AddSystem_Then_NoSystemAdded(Type type)
	{
		var component = new AtlasSystemRunner();

		Assert.That(!component.Add(type));
		Assert.That(component.Types.Count == 0);
	}

	[Test]
	public void When_AddSystem_Then_SystemAddedToEngine()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();

		var component = new AtlasSystemRunner(typeof(ITestSystem));


		root.AddComponent<ISystemRunner>(component);
		root.AddComponent<IEngine>(engine);



		Assert.That(engine.Systems.Has<ITestSystem>());
	}
	#endregion

	#region Remove
	[TestCase<TestSystem>]
	[TestCase<TestVariableFamilySystem>]
	[TestCase<TestMultipleSystem1>]
	[TestCase<TestMultipleSystem2>]
	public void When_RemoveSystem_Then_SystemRemoved<T>()
		where T : class, ISystem, new()
	{
		var component = new AtlasSystemRunner();
		component.Add<T>();
		component.Remove<T>();

		Assert.That(component.Types.Count == 0);
		Assert.That(!component.Has<T>());
	}

	[TestCase(null)]
	[TestCase(typeof(IComponent))]
	[TestCase(typeof(ISystem))]
	public void When_RemoveSystem_Then_NoSystemRemoved(Type type)
	{
		var component = new AtlasSystemRunner();

		Assert.That(!component.Remove(type));
		Assert.That(component.Types.Count == 0);
	}

	[Test]
	public void When_RemoveSystems_Then_SystemsRemoved()
	{
		var component = new AtlasSystemRunner();
		component.Add<TestSystem>();
		component.Add<TestVariableFamilySystem>();

		Assert.That(component.RemoveAll());
		Assert.That(component.Types.Count == 0);
	}

	[Test]
	public void When_RemoveSystems_Then_NoSystemsRemoved()
	{
		var component = new AtlasSystemRunner();

		Assert.That(!component.RemoveAll());
		Assert.That(component.Types.Count == 0);
	}

	[Test]
	public void When_RemoveSystem_Then_SystemRemovedFromEngine()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();
		var component = new AtlasSystemRunner(typeof(ITestSystem));

		root.AddComponent<ISystemRunner>(component);
		root.AddComponent<IEngine>(engine);

		root.RemoveComponent<ISystemRunner>();

		Assert.That(!engine.Systems.Has<ITestSystem>());
	}

	[Test]
	public void When_RemoveSystem_InDifferentStates_Then_SystemRemovedFromEngine()
	{
		var root = new AtlasEntity(true);
		var engine = new AtlasEngine();
		var component = new AtlasSystemRunner();

		//Component
		component.Add<ITestSystem>();
		Assert.That(component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 1);
		component.Remove<ITestSystem>();
		Assert.That(!component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 0);

		//Component & Entity
		root.AddComponent<ISystemRunner>(component);

		component.Add<ITestSystem>();
		Assert.That(component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 1);
		component.Remove<ITestSystem>();
		Assert.That(!component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 0);

		//Component & Entity & Engine
		root.AddComponent<IEngine>(engine);

		component.Add<ITestSystem>();
		Assert.That(component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 1);
		Assert.That(engine.Systems.Has<ITestSystem>());
		Assert.That(engine.Systems.VariableSystems.Count == 1);
		component.Remove<ITestSystem>();
		Assert.That(!component.Has<ITestSystem>());
		Assert.That(component.Types.Count == 0);
		Assert.That(!engine.Systems.Has<ITestSystem>());
		Assert.That(engine.Systems.VariableSystems.Count == 0);
	}
	#endregion
}