using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Systems;

[TestFixture]
class SystemGetterTests
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
		Assert.That(() => SystemGetter.GetSystem<T>(), expected ? Throws.Nothing : Throws.Exception);
	}

	[TestCase(typeof(ISystem), false)]
	[TestCase(typeof(ITestSystem), true)]
	[TestCase(typeof(TestSystem), true)]
	public void When_GetSystem_AsType_Then_HasSystem(Type type, bool expected)
	{
		Assert.That(() => SystemGetter.GetSystem(type), expected ? Throws.Nothing : Throws.Exception);

	}

	[TestCase<ITestMultipleSystem>]
	public void When_GetSystem_HasMultipleSystems_Then_ThrowsException<T>()
		where T : class, ISystem
	{
		Assert.That(() => SystemGetter.GetSystem<T>(), Throws.Exception);
	}
}