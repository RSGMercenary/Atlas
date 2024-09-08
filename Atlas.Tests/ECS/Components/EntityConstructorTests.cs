using Atlas.ECS.Components.EntityConstructor;
using Atlas.ECS.Entities;
using Atlas.Tests.Testers.Components;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Components;

[TestFixture]
internal class EntityConstructorTests
{
	[Test]
	public void When_Construct_Then_Constructed()
	{
		var entity = new AtlasEntity();
		var constructor = new TestEntityContructor();

		entity.AddComponent(constructor);

		Assert.That(entity.HasComponent<TestComponent>());
	}

	[Test]
	public void When_AutoRemove_Then_AutoRemoveExpected([Values(true, false)] bool autoRemove)
	{
		var constructor = new TestEntityContructor();

		constructor.AutoRemove = autoRemove;

		Assert.That(constructor.AutoRemove == autoRemove);
	}

	[TestCase(true, Construction.Deconstructed)]
	[TestCase(false, Construction.Constructed)]
	public void When_AutoRemove_Then_PropertiesExpected(bool autoRemove, Construction construction)
	{
		var entity = new AtlasEntity();
		var constructor = new TestEntityContructor(autoRemove);

		entity.AddComponent(constructor);

		Assert.That(entity.HasComponent<TestEntityContructor>() != autoRemove);
		Assert.That(constructor.Construction == construction);
	}

	[Test]
	public void When_Construct_Then_ConstructionExpected([Values(true, false)] bool autoRemove)
	{
		var entity = new AtlasEntity();
		var constructor = new TestEntityContructor(autoRemove);

		bool deconstructed = false;
		bool constructing = false;
		bool constructed = false;

		constructor.ConstructionChanged += (_, construction, _) =>
		{
			if(construction == Construction.Deconstructed)
				deconstructed = true;
			if(construction == Construction.Constructing)
				constructing = true;
			if(construction == Construction.Constructed)
				constructed = true;
		};

		entity.AddComponent(constructor);

		Assert.That(deconstructed == autoRemove);
		Assert.That(constructing);
		Assert.That(constructed);
	}
}