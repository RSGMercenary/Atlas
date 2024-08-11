using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Systems;

[TestFixture]
public class SystemGetterTests
{
	[TestCase<ISystem>(false)]
	[TestCase<ITestSystem>(true)]
	[TestCase<TestSystem>(true)]
	public void When_GetSystems_AsGeneric_Then_HasSystems<T>(bool expected)
		where T : class, ISystem
	{
		var systems = SystemGetter.GetSystems<T>();

		Assert.That(systems.Any() == expected);
	}

	[TestCase(typeof(ISystem), false)]
	[TestCase(typeof(ITestSystem), true)]
	[TestCase(typeof(TestSystem), true)]
	public void When_GetSystems_AsType_Then_HasSystems(Type type, bool expected)
	{
		var systems = SystemGetter.GetSystems(type);

		Assert.That(systems.Any() == expected);
	}

	[TestCase<ISystem>(false)]
	[TestCase<ITestSystem>(true)]
	[TestCase<TestSystem>(true)]
	public void When_GetSystem_AsGeneric_Then_HasSystem<T>(bool expected)
		where T : class, ISystem
	{
		var system = SystemGetter.GetSystem<T>();

		Assert.That(system != null == expected);
	}

	[TestCase(typeof(ISystem), false)]
	[TestCase(typeof(ITestSystem), true)]
	[TestCase(typeof(TestSystem), true)]
	public void When_GetSystem_AsType_Then_HasSystem(Type type, bool expected)
	{
		var system = SystemGetter.GetSystem(type);

		Assert.That(system != null == expected);
	}

	[TestCase<ITestMultipleSystem>]
	public void When_GetSystem_HasMultipleSystems_Then_ThrowsException<T>()
		where T : class, ISystem
	{
		Assert.That(() => SystemGetter.GetSystem<T>(), Throws.Exception);
	}
}