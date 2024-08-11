using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Components.Components;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components;

[TestFixture]
public class ComponentTests
{
	#region Add
	[Test]
	public void When_AddComponent_As_Component_Then_ComponentAdded()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);

		Assert.That(entity.GetComponent(component.GetType()) == component);
	}

	[TestCase(typeof(TestComponent))]
	[TestCase(typeof(ITestComponent))]
	public void When_AddComponent_As_Type_Then_ComponentAdded(Type type)
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component, type);

		Assert.That(entity.GetComponent(type) == component);
	}

	[TestCase<ITestComponent, TestComponent>]
	[TestCase<TestComponent, TestComponent>]
	public void When_AddComponent_As_Type_Then_ComponentAdded<TType, TComponent>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		var entity = new AtlasEntity();
		var component = new TComponent();

		entity.AddComponent<TType>(component);

		Assert.That(entity.GetComponent<TType>() == component);
	}

	[TestCase<ITestComponent, TestComponent>]
	[TestCase<TestComponent, TestComponent>]
	public void When_AddComponent_As_TypeAndComponent_Then_ComponentAdded<TType, TComponent>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		var entity = new AtlasEntity();
		var component = new TComponent();

		entity.AddComponent<TType, TComponent>(component);

		Assert.That(entity.GetComponent<TType>() == component);
	}

	[TestCase<TestComponent>]
	public void When_AddComponent_As_New_Then_ComponentAdded<TComponent>()
		where TComponent : class, IComponent, new()
	{
		var entity = new AtlasEntity();
		var component = entity.AddComponent<TComponent>();

		Assert.That(entity.GetComponent<TComponent>() == component);
	}

	[TestCase<ITestComponent, TestComponent>]
	public void When_AddComponent_As_TypeAndNew_Then_ComponentAdded<TType, TComponent>()
		where TType : class, IComponent
		where TComponent : class, IComponent, new()
	{
		var entity = new AtlasEntity();
		var component = entity.AddComponent<TComponent>();

		Assert.That(entity.GetComponent<TComponent>() == component);
	}

	[Test]
	public void When_AddComponent_TwiceSame_Then_ComponentSame()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();
		var type = component.GetType();

		entity.AddComponent(component);
		entity.AddComponent(component);

		Assert.That(entity.GetComponent(type) == component);
	}

	[Test]
	public void When_AddComponent_TwiceDifferent_Then_ComponentDifferent()
	{
		var entity = new AtlasEntity();
		var component1 = new TestComponent();
		var component2 = new TestComponent();
		var type = component1.GetType();

		entity.AddComponent(component1);
		entity.AddComponent(component2);

		Assert.That(entity.GetComponent(type) == component2);
	}
	#endregion

	#region Remove
	[Test]
	public void When_RemoveComponent_AsComponent_Then_ComponentRemoved()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();
		var type = component.GetType();

		entity.AddComponent(component);
		entity.RemoveComponent(type);

		Assert.That(entity.GetComponent(type) == null);
	}

	[TestCase(typeof(TestComponent))]
	[TestCase(typeof(ITestComponent))]
	public void When_RemoveComponent_AsType_Then_ComponentRemoved(Type type)
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component, type);
		entity.RemoveComponent(type);

		Assert.That(entity.GetComponent(type) == null);
	}

	[Test]
	public void When_RemoveComponent_Then_()
	{
		var entity = new AtlasEntity();

		Assert.That(entity.RemoveComponent<TestComponent>() == null);
	}

	[TestCase<ITestComponent, TestComponent>]
	[TestCase<TestComponent, TestComponent>]
	public void When_RemoveComponent_Then_ComponentRemoved<TType, TComponent>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		var entity = new AtlasEntity();
		var component = new TComponent();

		entity.AddComponent<TType, TComponent>(component);
		entity.RemoveComponent<TType>();

		Assert.That(entity.GetComponent<TType>() == null);
	}

	[Test]
	public void When_RemoveComponents_Then_ComponentsRemoved()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		entity.RemoveComponents();

		Assert.That(entity.Components.Count == 0);
	}
	#endregion

	#region Engine
	[TestCase(true)]
	[TestCase(false)]
	public void When_AddComponentEngine_With_IsRoot_Then_ThrowsExpected(bool isRoot)
	{
		var entity = new AtlasEntity(isRoot);
		var engine = new AtlasEngine();

		Assert.That(() => entity.AddComponent<IEngine>(engine), isRoot ? Throws.Nothing : Throws.Exception);
	}
	#endregion

	#region Dispose
	[TestCase(true)]
	[TestCase(false)]
	public void When_Dispose_With_IsAutoDisposable_Then_ComponentDisposed(bool isAutoDisposable)
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		component.IsAutoDisposable = isAutoDisposable;
		entity.AddComponent(component);
		entity.RemoveComponent(component);

		Assert.That(component.IsAutoDisposable == isAutoDisposable);
		Assert.That(component.TestDispose == isAutoDisposable);
	}
	#endregion
}