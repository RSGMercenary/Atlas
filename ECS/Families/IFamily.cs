using Atlas.Core.Collections.Group;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using System;

namespace Atlas.ECS.Families
{
	public interface IReadOnlyFamily : IObject<IReadOnlyFamily>
	{
		IReadOnlyGroup<IFamilyMember> Members { get; }
	}

	public interface IReadOnlyFamily<TFamilyMember> : IReadOnlyFamily
		where TFamilyMember : IFamilyMember, new()
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
		where TFamilyMember : IFamilyMember, new()
	{

	}


}