using Atlas.ECS.Components.Engine;
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

	#region Dispose
	[Test]
	public void When_Dispose_And_HasEngine_Then_NotDisposed()
	{
		var engine = new AtlasEngine();
		var family = engine.AddFamily<TestFamilyMember>();
		family.Dispose();

		Assert.That(family.Engine == engine);
	}
	#endregion

	#region Add
	[Test]
	public void When_AddEntity_Then_MemberAdded()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That((Family as IFamily).GetMember(entity) != null);
		Assert.That((Family as IReadOnlyFamily).Members.Count == 1);
		Assert.That(Family.HasMember(entity));
		Assert.That(member.Entity == entity);
		Assert.That(member.Component == component);
	}

	[Test]
	public void When_AddEntity_Twice_Then_MemberAdded()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);
		Family.AddEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That(Family.HasMember(entity));
		Assert.That(Family.Members.Count == 1);
		Assert.That(member.Entity == entity);
		Assert.That(member.Component == component);
	}
	#endregion

	#region Remove
	[Test]
	public void When_RemoveEntity_Then_MemberRemoved()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);
		Family.RemoveEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That((Family as IFamily).GetMember(entity) == null);
		Assert.That((Family as IReadOnlyFamily).Members.Count == 0);
		Assert.That(!Family.HasMember(entity));
		Assert.That(member == null);
	}

	[Test]
	public void When_RemoveEntity_Twice_Then_MemberRemoved()
	{
		var entity = new AtlasEntity();
		var component = new TestComponent();

		entity.AddComponent(component);
		Family.AddEntity(entity);
		Family.RemoveEntity(entity);
		Family.RemoveEntity(entity);

		var member = Family.GetMember(entity);

		Assert.That(!Family.HasMember(entity));
		Assert.That(Family.Members.Count == 0);
		Assert.That(member == null);
	}
	#endregion

	#region Get
	[Test]
	public void When_IterateMembers_Then_ContainsMembers()
	{
		var entities = Enumerable.Range(0, 10).Select(i => new AtlasEntity()).ToList();
		entities.ForEach(Family.AddEntity);
		foreach(IFamilyMember member in (Family as IReadOnlyFamily))
			Assert.That(entities.Contains(member.Entity));
	}
	#endregion
}