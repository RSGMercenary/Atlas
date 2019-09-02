using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Objects;
using Atlas.ECS.Components;
using Atlas.ECS.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Entities
{
	public interface IEntity : IObject<IEntity>, IHierarchy<IEntity>, IAutoDispose, ISleep
	{
		#region Entities

		/// <summary>
		/// Determines whether <see cref="IObject.Dispose"/> is called when <see cref="Parent"/> == <see langword="null"/>.
		/// </summary>
		new bool AutoDispose { get; set; }

		/// <summary>
		/// This <see cref="IEntity"/>'s global name. This name is unique to its Engine.
		/// If this Entity is added to an Engine, and this global name already
		/// exists, then this Entity's global name will be changed.
		/// </summary>
		string GlobalName { get; set; }

		/// <summary>
		/// This <see cref="IEntity"/>'s local name. This name is unique to its Parent.
		/// If this Entity is added to a parent, and this local name already
		/// exists, then this Entity's local name will be changed.
		/// </summary>
		string LocalName { get; set; }

		IEntity AddChild(string globalName, string localName);
		IEntity AddChild(string globalName, string localName, int index);
		IEntity AddChild(string globalName, bool localName = false);
		IEntity AddChild(string globalName, bool localName, int index);

		bool HasChild(string localName);

		/// <summary>
		/// Returns the <see cref="IEntity"/> in the given <paramref name="hierarchy"/>, starting
		/// with this <see cref="IEntity"/> and traversing parents and/or children until the right
		/// <see cref="IEntity"/> is found.
		/// <para>".." is the parent, and "/" is the name separator.</para>
		/// </summary>
		/// <param name="hierarchy">A hierarchy string in format of "../../name1/name2/name3".</param>
		/// <returns></returns>
		IEntity GetHierarchy(string hierarchy);

		IEntity SetHierarchy(string hierarchy, int index);

		IEntity GetChild(string localName);

		IEntity RemoveChild(string localName);

		#endregion

		#region Components

		#region Has

		bool HasComponent<TKey>()
			where TKey : IComponent;

		bool HasComponent(Type type);

		#endregion

		#region Get

		TValue GetComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey;

		TKeyValue GetComponent<TKeyValue>()
			where TKeyValue : IComponent;

		IComponent GetComponent(Type type);

		TValue GetComponent<TValue>(Type type)
			where TValue : IComponent;

		Type GetComponentType(IComponent component);

		IReadOnlyDictionary<Type, IComponent> Components { get; }

		TKeyValue GetAncestorComponent<TKeyValue>()
			where TKeyValue : IComponent;

		IEnumerable<TKeyValue> GetDescendantComponents<TKeyValue>()
			where TKeyValue : IComponent;

		#endregion

		#region Add

		/// <summary>
		/// Adds a Component to the Entity with a new instance.
		/// </summary>
		/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
		/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
		/// <returns></returns>
		TValue AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey, new();

		/// <summary>
		/// Adds a Component to the Entity with the given instance.
		/// </summary>
		/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
		/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TValue AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : TKey;

		/// <summary>
		/// Adds a Component to the Entity with the given instance and index.
		/// </summary>
		/// <typeparam name="TKey">The interface Type of the Component.</typeparam>
		/// <typeparam name="TValue">The instance Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <param name="index">The index of the Entity within the Component.</param>
		/// <returns></returns>
		TValue AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : TKey;

		/// <summary>
		/// Adds a Component to the Entity with a new instance.
		/// </summary>
		/// <typeparam name="TKeyValue">The instance Type of the Component.</typeparam>
		/// <returns></returns>
		TKeyValue AddComponent<TKeyValue>()
			where TKeyValue : IComponent, new();

		/// <summary>
		/// Adds a Component to the Entity with the given instance.
		/// </summary>
		/// <typeparam name="TKeyValue">The interface Type of the Component.</typeparam>
		/// <param name="Component">The instance of the Component.</param>
		/// <returns></returns>
		TKeyValue AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : IComponent;

		/// <summary>
		/// Adds a Component to the Entity with the given instance and index.
		/// </summary>
		/// <typeparam name="TKeyValue">The interface Type of the Component.</typeparam>
		/// <param name="component">The instance of the Component.</param>
		/// <param name="index">The index of the Entity within the Component.</param>
		/// <returns></returns>
		TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : IComponent;

		TValue AddComponent<TValue>(TValue component, Type type)
			where TValue : IComponent;

		TValue AddComponent<TValue>(Type type)
			where TValue : IComponent, new();

		IComponent AddComponent(IComponent component);
		IComponent AddComponent(IComponent component, Type type);
		IComponent AddComponent(IComponent component, int index);
		IComponent AddComponent(IComponent component, Type type = null, int index = int.MaxValue);

		#endregion

		#region Remove

		TValue RemoveComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey;

		TKeyValue RemoveComponent<TKeyValue>()
			where TKeyValue : IComponent;

		IComponent RemoveComponent(Type type);

		IComponent RemoveComponent(IComponent component);

		bool RemoveComponents();

		#endregion

		#endregion

		#region Sleeping

		/// <summary>
		/// Return whether this Entity.FreeSleeping > 0;
		/// </summary>
		bool IsFreeSleeping { get; set; }

		/// <summary>
		/// This Entity's free-sleeping count. If this Entity is free-sleeping,
		/// its sleeping is not changed by its parent sleeping.
		/// </summary>
		int FreeSleeping { get; }

		#endregion

		#region Info Strings

		string AncestorsToString(int depth = -1, bool localNames = true, string indent = "");

		string DescendantsToString(int depth = -1, bool localNames = true, string indent = "");

		/// <summary>
		/// Returns a formatted and indented string of the <see cref="IEntity"/> hierarchy.
		/// </summary>
		/// <param name="depth">Adds children recursively to the output until the given depth. -1 is the entire hierarchy.</param>
		/// <param name="addComponents">Adds components to the output.</param>
		/// <param name="addManagers">Adds component entities to the output.</param>
		/// <param name="addSystems">Adds systems to the output.</param>
		/// <param name="indent"></param>
		/// <returns></returns>
		string ToInfoString(int depth = -1, bool addComponents = true, bool addManagers = false, string indent = "", StringBuilder text = null);

		#endregion
	}
}