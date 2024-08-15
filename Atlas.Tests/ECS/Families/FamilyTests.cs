using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.Tests.ECS.Components.Components;
using Atlas.Tests.ECS.Families.Families;
using NUnit.Framework;

namespace Atlas.Tests.ECS.Families;

[TestFixture]
class FamilyTests
{
	public AtlasFamily<TestFamilyMember> Family;

	[SetUp]
	public void SetUp()
	{
		Family = new();
	}

	[Test]
	public void When_AddEntity_Then_MemberAdded()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That(Family.HasMember(entity));
		Assert.That(Family.Members.Count == 1);
		Assert.That(member.Entity == entity);
		Assert.That(member.Component == component);
	}

	[Test]
	public void When_RemoveEntity_Then_MemberRemoved()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);
		Family.RemoveEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That(!Family.HasMember(entity));
		Assert.That(Family.Members.Count == 0);
		Assert.That(member == null);
	}
}