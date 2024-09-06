using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.Tests.Testers.Components;
using Atlas.Tests.Testers.Families;
using NUnit.Framework;
using System;
using System.Linq;

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
		var family = engine.Families.Add<TestFamilyMember>();
		//family.Dispose();

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

	[Test]
	public void When_InsertionSort_Then_MembersSorted()
	{
		var count = 200;

		AddEntities(count);

		Family.InsertionSort((m1, m2) => string.Compare(m1.Entity.GlobalName, m2.Entity.GlobalName));

		AssertAlphabetical(count);
	}

	[Test]
	public void When_MergeSort_Then_MembersSorted()
	{
		var count = 200;

		AddEntities(count);

		Family.MergeSort((m1, m2) => string.Compare(m1.Entity.GlobalName, m2.Entity.GlobalName));

		AssertAlphabetical(count);
	}

	private void AddEntities(int count)
	{
		for(int i = 0; i < count; ++i)
		{
			var entity = new AtlasEntity();
			var component = new TestComponent();

			entity.AddComponent(component);

			Family.AddEntity(entity);
		}
	}

	private void AssertAlphabetical(int count)
	{
		var members = Family.Members;
		var alphabetical = true;
		for(var node = members.First; node.Next != null; node = node.Next)
		{
			var name1 = node.Value.Entity.GlobalName;
			var name2 = node.Next.Value.Entity.GlobalName;
			--count;

			if(name1.CompareTo(name2) > 0)
			{
				alphabetical = false;
				break;
			}
		}

		Assert.That(alphabetical);
		Assert.That(--count == 0);
	}
}