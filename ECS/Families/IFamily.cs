using Atlas.Core.Collections.Group;
using Atlas.Core.Utilites;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Families
{
	public interface IReadOnlyFamily : IObject, IEnumerable
	{
		/// <summary>
		/// Automatically called on Families removed from the Engine.
		/// </summary>
		new void Dispose();

		IReadOnlyGroup<IFamilyMember> Members { get; }

		IFamilyMember GetMember(IEntity entity);
	}

	public interface IFamily : IReadOnlyFamily
	{
		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);
	}

	public interface IReadOnlyFamily<TFamilyMember> : IReadOnlyFamily, IObject<IReadOnlyFamily<TFamilyMember>>, IEnumerable<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		new IReadOnlyGroup<TFamilyMember> Members { get; }

		new TFamilyMember GetMember(IEntity entity);

		void SortMembers(Sort sort, Func<TFamilyMember, TFamilyMember, int> compare);
	}

	public interface IFamily<TFamilyMember> : IReadOnlyFamily<TFamilyMember>, IFamily
		where TFamilyMember : class, IFamilyMember, new()
	{
	}
}