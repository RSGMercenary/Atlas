using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.Testers.Systems;
using NUnit.Framework;
using System;

namespace Atlas.Tests.ECS.Components.Engine;

[TestFixture]
internal class SystemManagerTests
{
	public AtlasEngine Engine;

	[SetUp]
	public void SetUp()
	{
		Engine = new AtlasEngine();
	}

	#region Add
	[TestCase<ITestFixedFamilySystem>]
	[TestCase<TestFixedFamilySystem>]
	public void When_AddFixedSystem_Then_SystemAdded<T>()
		where T : class, ISystem
	{
		var system = Engine.Systems.Add<T>();

		Assert.That(Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get<T>() == system);
		Assert.That(Engine.Systems.FixedSystems.Count == 1);
	}

	[TestCase<ITestVariableFamilySystem>]
	[TestCase<TestVariableFamilySystem>]
	public void When_AddVariableSystem_Then_SystemAdded<T>()
		where T : class, ISystem
	{
		var system = Engine.Systems.Add<T>();

		Assert.That(Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get<T>() == system);
		Assert.That(Engine.Systems.VariableSystems.Count == 1);
	}

	[TestCase(typeof(ITestSystem))]
	[TestCase(typeof(TestSystem))]
	public void When_AddSystem_Then_SystemAdded(Type type)
	{
		var system = Engine.Systems.Add(type);

		Assert.That(Engine.Systems.Has(type));
		Assert.That(Engine.Systems.Has(system));
		Assert.That(Engine.Systems.Get(type) == system);
		Assert.That(Engine.Systems.VariableSystems.Count == 1);
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
		Assert.That(Engine.Systems.VariableSystems.Count == 1);
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
		Assert.That(engine.Systems.VariableSystems.Count == 0);
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
		Assert.That(engine.Systems.VariableSystems.Count == 0);
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
		Assert.That(Engine.Systems.VariableSystems.Count == 0);
	}

	[TestCase<ITestSystem>]
	public void When_RemoveSystem_With_NoSystem_Then_SystemRemoved<T>()
		where T : class, ISystem
	{
		Engine.Systems.Remove<T>();

		Assert.That(!Engine.Systems.Has<T>());
		Assert.That(Engine.Systems.Get<T>() == null);
		Assert.That(Engine.Systems.VariableSystems.Count == 0);
	}
	#endregion

	#region Get
	[TestCase(true)]
	[TestCase(false)]
	public void When_GetSystem_As_Index_Then_ThrowsExpected(bool addSystem)
	{
		if(addSystem)
			Engine.Systems.Add<ITestSystem>();

		Assert.That(() => Engine.Systems.Get(TimeStep.Variable, 0), addSystem ? Throws.Nothing : Throws.Exception);
	}
	#endregion

	#region Priority
	[Test]
	public void When_PriorityChanged_Then_SystemsPrioritized()
	{
		var system1 = Engine.Systems.Add<TestMultipleSystem1>();
		var system2 = Engine.Systems.Add<TestMultipleSystem2>();

		system2.Priority = -1;

		Assert.That(Engine.Systems.VariableSystems[0] == system2);
		Assert.That(Engine.Systems.VariableSystems[1] == system1);
	}
	#endregion
}