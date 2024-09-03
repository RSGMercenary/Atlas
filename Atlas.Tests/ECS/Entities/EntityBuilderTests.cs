using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.Tests.Attributes;
using Atlas.Tests.Testers.Components;
using NUnit.Framework;
using System;

namespace Atlas.Tests.ECS.Entities;

[TestFixture]
internal class EntityBuilderTests
{
	private AtlasEntity Entity;

	[SetUp]
	public void SetUp()
	{
		Entity = new AtlasEntity();
	}

	[Test]
	public void When_BuildEntity_Then_Built(
		[Values("Global", "Name")] string globalName,
		[Values("Local", "Name")] string localName,
		[Values(true, false)] bool addChild,
		[Values(true, false)] bool setParent,
		[Values(true, false)] bool addComponent,
		[Values(true, false)] bool sleep,
		[Values(true, false)] bool selfSleep,
		[Values(true, false)] bool autoDispose)
	{
		var parent = new AtlasEntity();
		var child = new AtlasEntity();

		var builder = Entity.Build()
			.GlobalName(globalName)
			.LocalName(localName)
			.Sleep(sleep)
			.SelfSleep(selfSleep)
			.AutoDispose(autoDispose);

		if(setParent)
			builder.SetParent(parent);
		if(addChild)
			builder.AddChild(child);
		if(addComponent)
			builder.AddComponent<TestComponent>();

		builder.Finish();

		Assert.That(Entity.GlobalName == globalName);
		Assert.That(Entity.LocalName == localName);
		Assert.That(Entity.IsSleeping == sleep);
		Assert.That(Entity.IsSelfSleeping == selfSleep);
		Assert.That(Entity.AutoDispose == autoDispose);
		Assert.That(Entity.Parent == parent == setParent);
		Assert.That(Entity.HasChild(child) == addChild);
		Assert.That(Entity.HasComponent<TestComponent>() == addComponent);
	}

	[Test]
	public void When_BuildRoot_Then_Built()
	{
		var entity = new AtlasEntity();

		Entity.Build()
			.IsRoot(true)
			.AddComponent<AtlasEngine>()
			.Finish();

		Assert.That(Entity.GlobalName == AtlasEntity.RootName);
		Assert.That(Entity.LocalName == AtlasEntity.RootName);
		Assert.That(Entity.HasComponent<AtlasEngine>());
	}

	[TestCase<TestComponent>(null)]
	[TestCase<TestComponent>(typeof(TestComponent))]
	[TestCase<TestComponent>(typeof(ITestComponent))]
	public void When_AddComponent_AsNew_Then_ComponentAdded<TComponent>(Type type)
		where TComponent : class, IComponent, new()
	{
		Entity.Build().AddComponent<TComponent>(type);

		Assert.That(Entity.HasComponent(type ?? typeof(TComponent)));
	}

	[TestCase<TestComponent>(null)]
	[TestCase<TestComponent>(typeof(TestComponent))]
	[TestCase<TestComponent>(typeof(ITestComponent))]
	public void When_AddComponent_AsInstance_Then_ComponentAdded<TComponent>(Type type)
		where TComponent : class, IComponent, new()
	{
		Entity.Build().AddComponent(new TComponent(), type);

		Assert.That(Entity.HasComponent(type ?? typeof(TComponent)));
	}

	[TestCase<TestComponent, ITestComponent>]
	[TestCase<TestComponent, TestComponent>]
	public void When_AddComponent_AsNew_Then_ComponentAdded<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		Entity.Build().AddComponent<TComponent, TType>();

		Assert.That(Entity.HasComponent<TType>());
	}

	[TestCase<TestComponent, ITestComponent>]
	[TestCase<TestComponent, TestComponent>]
	public void When_AddComponent_AsInstance_Then_ComponentAdded<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		Entity.Build().AddComponent<TComponent, TType>(new TComponent());

		Assert.That(Entity.HasComponent<TType>());
	}
}