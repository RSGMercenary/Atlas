using Atlas.Core.Extensions;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Entities;
using Atlas.Tests.Attributes;
using Atlas.Tests.ECS.Components.Components;
using Atlas.Tests.ECS.Families.Families;
using Atlas.Tests.ECS.Systems.Systems;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace Atlas.Tests.Core.Extensions;

[TestFixture]
class ReflectionExtensionsTests
{
	private const BindingFlags Flags = BindingFlags.NonPublic | BindingFlags.Instance;

	[TestCase<TestFamilyMember>("<Component>k__BackingField")]
	[TestCase<TestFamilyMember>("<Entity>k__BackingField")]
	public void When_Findfield_Then_FieldExpected<T>(string fieldName)
	{
		var result = typeof(T).FindField(fieldName, Flags);

		Assert.That(result.Name == fieldName);
	}

	[TestCase<TestFamilyMember, IComponent>(true)]
	[TestCase<TestFamilyMember, IEntity>(true)]
	[TestCase<TestSystem, IEntity>(false)]
	public void When_FindFields_WithGeneric_Then_FieldsExpected<TInstance, TField>(bool expected)
	{
		var result = typeof(TInstance).FindFields<TField>(Flags).Any();

		Assert.That(result == expected);
	}

	[TestCase<TestFamilyMember, IEntity>(true)]
	[TestCase<TestFamilyMember, TestComponent>(true)]
	[TestCase<TestSystem, IEntity>(false)]
	public void When_FindField_WithGeneric_Then_FieldExpected<TInstance, TType>(bool expected)
	{
		var result = typeof(TInstance).FindField<TType>(Flags);

		Assert.That(result != null == expected);
	}

	[Test]
	public void When_FindField_WithMultipleFields_Then_ThrowsException()
	{
		var method = () => typeof(TestComponent).FindField<TestComponent>(Flags);

		Assert.That(method, Throws.Exception);
	}

	[Test]
	public void When_FindField_IsNull_Then_()
	{
		var result = (null as Type).FindField("name", Flags);

		Assert.That(result == null);
	}
}