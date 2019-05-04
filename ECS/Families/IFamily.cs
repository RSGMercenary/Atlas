using Atlas.Core.Collections.Group;
using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Families
{
	public interface IFamily : IObject, IEnumerable
	{
		IReadOnlyGroup<IFamilyMember> Members { get; }

		IFamilyMember GetMember(IEntity entity);

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);
	}

	public interface IFamily<TFamilyMember> : IFamily, IEnumerable<TFamilyMember>
		where TFamilyMember : class, IFamilyMember, new()
	{
		new IReadOnlyGroup<TFamilyMember> Members { get; }

		new TFamilyMember GetMember(IEntity entity);

		/// <summary>
		/// Sorts the Members based on sorting algorithms and comparing methods.
		/// Sorting algorithms in <see cref="Core.Utilites.Sort"/> can be used depending on
		/// the performance needs of a given System.
		/// <para>This should be done before beginning to update Members.</para>
		/// </summary>
		/// <param name="sort"></param>
		/// <param name="compare"></param>
		void Sort(Action<IList<TFamilyMember>, Func<TFamilyMember, TFamilyMember, int>> sort, Func<TFamilyMember, TFamilyMember, int> compare);
	}
}