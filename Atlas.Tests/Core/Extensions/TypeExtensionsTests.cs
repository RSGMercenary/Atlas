using Atlas.Core.Extensions;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Systems;
using Atlas.Tests.Attributes;
using Atlas.Tests.Core.Extensions.Classes;
using Atlas.Tests.ECS.Components.Components;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;

namespace Atlas.Tests.Core.Extensions;

[TestFixture]
class TypeExtensionsTests
{
	[TestCase<IComponent, TestComponent, ITestComponent>(false)]
	[TestCase<ITestComponent, TestComponent, ITestComponent>(true)]
	[TestCase<ISystem, TestSystem, ITestSystem>(false)]
	[TestCase<ISystem, TestFamilySystem, ITestFamilySystem>(false)]
	[TestCase<IComponent, AtlasEngine, IEngine>(false)]
	[TestCase<IEntity, AtlasEntity, IEntity>(true)]
	public void When_GetInterfaceType_Then_TypeExpected<TType, TInstance, TExpected>(bool inclusive)
		where TType : class
		where TInstance : class, TType, new()
	{
		Assert.That(new TInstance().GetInterfaceType<TType>(inclusive) == typeof(TExpected));
	}

	[TestCase<TestBaseClass>]
	[TestCase<TestSubClass>]
	public void when_GetInterfaceType_And_ManyResults_Then_ThrowsException<T>()
		where T : ITestBaseInterface, new()
	{
		Assert.That(() => new T().GetInterfaceType<ITestBaseInterface>(), Throws.Exception);
	}

	[TestCase<ITestSubInterface1, TestBaseClass>]
	[TestCase<ITestSubInterface2, TestSubClass>]
	public void when_GetInterfaceType_And_NoResults_Then_NullExpected<TType, TInstance>()
		where TType : class
		where TInstance : class, TType, new()
	{
		Assert.That(new TInstance().GetInterfaceType<TType>(false) == null);
	}
}