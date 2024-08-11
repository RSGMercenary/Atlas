using Atlas.Core.Collections.Group;
using Atlas.Core.Messages;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Entities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Families;

public interface IReadOnlyFamily : IMessenger, IEngineItem, IEnumerable
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

public interface IReadOnlyFamily<TFamilyMember> : IMessenger<IReadOnlyFamily<TFamilyMember>>, IReadOnlyFamily, IEnumerable<TFamilyMember>
	where TFamilyMember : class, IFamilyMember, new()
{
	new IReadOnlyGroup<TFamilyMember> Members { get; }

	new TFamilyMember GetMember(IEntity entity);

	void SortMembers(Action<IList<TFamilyMember>, Func<TFamilyMember, TFamilyMember, int>> sorter, Func<TFamilyMember, TFamilyMember, int> compare);
}

public interface IFamily<TFamilyMember> : IReadOnlyFamily<TFamilyMember>, IFamily
	where TFamilyMember : class, IFamilyMember, new()
{
}