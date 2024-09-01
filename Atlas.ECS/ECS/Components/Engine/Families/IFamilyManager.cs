using Atlas.Core.Collections.LinkList;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Families;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IFamily"/> management.
/// </summary>
public interface IFamilyManager : IReadOnlyEngineManager
{
	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="IFamily"/> is added.
	/// </summary>
	event Action<IFamilyManager, IFamily> Added;

	/// <summary>
	/// The <see langword="event"/> invoked when an <see cref="IFamily"/> is removed.
	/// </summary>
	event Action<IFamilyManager, IFamily> Removed;

	/// /// <summary>
	/// All <see cref="IFamily"/> instances managed by the <see cref="IFamilyManager"/>.
	/// <para><see cref="IFamily"/> instances are added/removed by <see cref="ISystem"/> instances.</para>
	/// </summary>
	IReadOnlyLinkList<IReadOnlyFamily> Families { get; }

	/// /// <summary>
	/// All <see cref="IFamily"/> instances by <see cref="Type"/> managed by the <see cref="IFamilyManager"/>.
	/// </summary>
	IReadOnlyDictionary<Type, IReadOnlyFamily> Types { get; }

	IFamilyCreator Creator { get; set; }

	/// <summary>
	/// Adds an <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/> to the <see cref="IFamilyManager"/>.
	/// </summary>
	/// <typeparam name="TFamilyMember">The <see cref="IFamilyMember"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	IReadOnlyFamily<TFamilyMember> Add<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Removes an <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/> from the <see cref="IFamilyManager"/>.
	/// </summary>
	/// <typeparam name="TFamilyMember">The <see cref="IFamilyMember"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	bool Remove<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Returns the <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/>.
	/// </summary>
	/// <typeparam name="TFamilyMember">The <see cref="IFamilyMember"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	IReadOnlyFamily<TFamilyMember> Get<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Returns the <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/>.
	/// </summary>
	/// <param name="type">The <see cref="IFamilyMember"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	IReadOnlyFamily Get(Type type);

	/// <summary>
	/// Returns if the <see cref="IFamilyManager"/> is managing a <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/>.
	/// </summary>
	/// <typeparam name="TFamilyMember">The <see cref="IFamilyMember"/> generic <see cref="Type"/>.</typeparam>
	/// <returns></returns>
	bool Has<TFamilyMember>()
		where TFamilyMember : class, IFamilyMember, new();

	/// <summary>
	/// Returns if the <see cref="IFamilyManager"/> is managing a <see cref="IFamily"/> with the given <see cref="IFamilyMember"/> <see cref="Type"/>.
	/// </summary>
	/// <param name="type">The <see cref="IFamilyMember"/> <see cref="Type"/>.</param>
	/// <returns></returns>
	bool Has(Type type);

	/// <summary>
	/// Returns if the <see cref="IFamilyManager"/> is managing a <see cref="IFamily"/> with the given instance.
	/// </summary>
	/// <param name="family">The <see cref="IFamily"/> instance.</param>
	/// <returns></returns>
	bool Has(IReadOnlyFamily family);
}