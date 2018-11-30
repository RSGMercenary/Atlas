using Atlas.Core.Collections.Group;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Families
{
	public interface IReadOnlyFamily : IObject, IEnumerable
	{
		IReadOnlyGroup<IFamilyMember> Members { get; }
	}

	public interface IReadOnlyFamily<TFamilyMember> : IReadOnlyFamily, IEnumerable<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		new IReadOnlyGroup<TFamilyMember> Members { get; }
	}

	public interface IFamily : IReadOnlyFamily
	{
		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);
	}

	public interface IFamily<TFamilyMember> : IFamily, IReadOnlyFamily<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{

	}
}