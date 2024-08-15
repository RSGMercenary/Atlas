using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Serialization;
using Atlas.ECS.Systems;
using Atlas.Tests.ECS.Components.Components;
using Atlas.Tests.ECS.Families.Families;
using Atlas.Tests.ECS.Systems.Systems;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Serialization;

[TestFixture]
class SerializationTests
{
	public Random Random;

	[SetUp]
	public void SetUp()
	{
		Random = new Random();
	}

	[Test]
	[Repeat(20)]
	public void When_SerializeEntity_Then_DeserializeEntity_IsEqual()
	{
		var root = new AtlasEntity(true);

		AddChildren(root, Random, Random.Next(6));

		var json = AtlasSerializer.Serialize(root);
		var clone = AtlasSerializer.Deserialize<IEntity>(json);

		Assert.That(root.Serialize() == clone.Serialize());
	}

	[TestCase]
	[Repeat(20)]
	public void When_SerializeEntity_WithDepth_Then_DeserializeEntity_IsEqual()
	{
		var root = new AtlasEntity(true);

		var depth = Random.Next(6);
		var maxDepth = Random.Next(-1, depth + 1);

		AddChildren(root, Random, depth);

		var json = AtlasSerializer.Serialize(root);
		var clone = AtlasSerializer.Deserialize<IEntity>(json);

		Assert.That(root.Serialize(Formatting.None, maxDepth) == clone.Serialize(Formatting.None, maxDepth));
	}

	private void AddChildren(IEntity entity, Random random, int depth)
	{
		entity.IsAutoDisposable = Random.NextBool();
		entity.IsSleeping = Random.NextBool();
		entity.IsSelfSleeping = Random.NextBool();

		if(random.NextBool())
			entity.AddComponent<TestComponent>();

		if(depth <= 0)
			return;

		for(int i = random.Next(6); i > 0; --i)
			AddChildren(entity.AddChild(new AtlasEntity()), random, --depth);
	}

	[Test]
	[Repeat(20)]
	public void When_SerializeComponent_Then_DeserializeComponent_IsEqual()
	{
		var component = new TestComponent();

		component.IsAutoDisposable = Random.NextBool();

		if(Random.NextBool())
		{
			component.IsShareable = true;
			for(int i = Random.Next(6); i > 0; --i)
				component.AddManager(new AtlasEntity());
		}
		else if(Random.NextBool())
			component.AddManager(new AtlasEntity());

		var json = AtlasSerializer.Serialize(component);
		var clone = AtlasSerializer.Deserialize<IComponent>(json);

		Assert.That(component.Serialize() == clone.Serialize());
	}

	[Test]
	[Repeat(20)]
	public void When_SerializeFamily_Then_DeserializeFamily_IsEqual()
	{
		var family = new AtlasFamily<TestFamilyMember>();

		for(int i = Random.Next(11); i > 0; --i)
		{
			var entity = new AtlasEntity();
			if(Random.NextBool())
				entity.AddComponent<TestComponent>();

			family.AddEntity(entity);
		}

		var json = AtlasSerializer.Serialize(family);
		var clone = AtlasSerializer.Deserialize<IFamily>(json);

		Assert.That(family.Serialize() == clone.Serialize());
	}

	[Test]
	[Repeat(20)]
	public void When_SerializeSystem_Then_DeserializeSystem_IsEqual()
	{
		var system = new TestFamilySystem();
		var family = new AtlasFamily<TestFamilyMember>();
		var timeSteps = new[] { TimeStep.None, TimeStep.Variable, TimeStep.Fixed };

		for(int i = Random.Next(6); i > 0; --i)
		{
			var entity = new AtlasEntity();
			entity.AddComponent<TestComponent>();
			family.AddEntity(entity);
		}

		system.Family = family;
		system.Priority = Random.Next(int.MinValue, int.MaxValue);
		system.IsSleeping = Random.NextBool();
		system.UpdateSleepingEntities = Random.NextBool();
		system.UpdateState = timeSteps[Random.Next(3)];
		system.UpdateStep = timeSteps[Random.Next(3)];

		var json = AtlasSerializer.Serialize(system);
		var clone = AtlasSerializer.Deserialize<ISystem>(json);

		Assert.That(system.Serialize() == clone.Serialize());
	}
}