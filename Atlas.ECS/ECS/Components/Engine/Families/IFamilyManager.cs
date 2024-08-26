using Atlas.Core.Collections.Group;
using Atlas.ECS.Families;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Families;

public interface IFamilyManager : IReadOnlyEngineManager
{
	event Action<IFamilyManager, IFamily> Added;

	event Action<IFamilyManager, IFamily> Removed;

	/// <summary>
	/// A collection of all Families managed by this Engine.
	/// 
	/// <para>Families of Entities are added to and removed from the Engine by
	/// being managed by a System intent on updating that Family.</para>
	/// </summary>
	IReadOnlyGroup<IReadOnlyFamily> Families { get; }

	IReadOnlyDictionary<Type, IReadOnlyFamily> Types { get; }

	/// <summary>
	/// Returns if the Engine is managing a Family with the given instance.
	/// </summary>
	/// <param name="family"></param>
	/// <returns></returns>
	bool Has(IReadOnlyFamily family);

	/// <summary>
	/// Returns if the Engine is managing a Family with the given Type.
	/// </summary>
	/// <typeparam name="TFamilyType"></typeparam>
	/// <returns></returns>
	bool Has<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Returns if the Engine is managing a Family with the given Type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	bool Has(Type type);

	/// <summary>
	/// Returns the Family with the given Type.
	/// </summary>
	/// <typeparam name="TFamilyMember"></typeparam>
	/// <returns></returns>
	IReadOnlyFamily<TFamilyMember> Get<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Returns the Family with the given Type.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	IReadOnlyFamily Get(Type type);

	IReadOnlyFamily<TFamilyMember> Add<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	void Remove<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();
}