using Atlas.Core.Collections.LinkList;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.Engine.Families;
using Atlas.ECS.Entities;
using Atlas.ECS.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Families;

public interface IReadOnlyFamily : IEngineManager<IReadOnlyFamily>, IEnumerable, IDisposable, ISerialize
{
	event Action<IReadOnlyFamily, IFamilyMember> MemberAdded;
	event Action<IReadOnlyFamily, IFamilyMember> MemberRemoved;

	/// <summary>
	/// Automatically called on <see cref="IFamily"/> instances removed from the <see cref="IFamilyManager"/>.
	/// </summary>
	new void Dispose();

	IReadOnlyLinkList<IFamilyMember> Members { get; }

	IFamilyMember GetMember(IEntity entity);
}

public interface IFamily : IReadOnlyFamily
{
	void AddEntity(IEntity entity);
	void RemoveEntity(IEntity entity);

	void AddEntity(IEntity entity, Type type);
	void RemoveEntity(IEntity entity, Type type);
}

public interface IReadOnlyFamily<TFamilyMember> : IEngineManager<IReadOnlyFamily>, IReadOnlyFamily, IEnumerable<TFamilyMember>
	where TFamilyMember : class, IFamilyMember, new()
{
	new event Action<IReadOnlyFamily<TFamilyMember>, TFamilyMember> MemberAdded;
	new event Action<IReadOnlyFamily<TFamilyMember>, TFamilyMember> MemberRemoved;

	new IReadOnlyLinkList<TFamilyMember> Members { get; }

	new TFamilyMember GetMember(IEntity entity);

	void SortMembers(Action<ILinkList<TFamilyMember>, Func<TFamilyMember, TFamilyMember, int>> sorter, Func<TFamilyMember, TFamilyMember, int> compare);
}

public interface IFamily<TFamilyMember> : IReadOnlyFamily<TFamilyMember>, IFamily
	where TFamilyMember : class, IFamilyMember, new()
{
}