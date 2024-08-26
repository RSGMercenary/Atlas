using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Entities;
using Atlas.ECS.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Component;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasComponent : AtlasComponent<AtlasComponent>
{
	#region Static
	internal static Type GetType(IComponent component, Type type = null)
	{
		if(type == null)
			type = component.GetType();
		else if(!type.IsInstanceOfType(component))
			throw new ArgumentException($"The component '{component.GetType()}' is not an instance of type '{type}'.");
		return type;
	}
	#endregion
}

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasComponent<T> : IComponent<T>, IEnumerable<IEntity>, ISerialize
	where T : class, IComponent<T>
{
	public event Action<IComponent, bool, bool> IsAutoDisposableChanged
	{
		add => AutoDispose.IsAutoDisposableChanged += value;
		remove => AutoDispose.IsAutoDisposableChanged -= value;
	}
	public event Action<IComponent, IEntity> ManagerAdded;
	public event Action<IComponent, IEntity> ManagerRemoved;
	public event Action<IComponent> ManagersChanged;


	#region Fields
	private readonly Group<IEntity> managers = new();
	private readonly AutoDispose<IComponent> AutoDispose;
	#endregion

	#region Construct / Dispose

	protected AtlasComponent() : this(false) { }

	protected AtlasComponent(bool isShareable)
	{
		IsShareable = isShareable;
		AutoDispose = new(this as T, () => managers.Count <= 0);
	}

	public virtual void Dispose()
	{
		Disposing();
	}

	protected virtual void Disposing()
	{
		RemoveManagers();
		IsAutoDisposable = true;

		PoolManager.Instance.Put(this);
	}

	#endregion

	[JsonProperty(Order = int.MinValue)]
	public bool IsShareable { get; private set; } = false;

	#region AutoDispose
	[JsonProperty(Order = int.MinValue + 1)]
	public bool IsAutoDisposable
	{
		get => AutoDispose.IsAutoDisposable;
		set => AutoDispose.IsAutoDisposable = value;
	}
	#endregion

	#region Managers
	#region Has
	public bool HasManager(IEntity entity) => managers.Contains(entity);
	#endregion

	#region Get
	public IEntity Manager => !IsShareable && managers.Count > 0 ? managers[0] : null;

	[JsonIgnore]
	public IReadOnlyGroup<IEntity> Managers => managers;

	[JsonProperty(PropertyName = nameof(Managers))]
	private IEnumerable<IEntity> SerializeManagers
	{
		get => Managers;
		set
		{
			foreach(var manager in value)
				AddManager(manager);
		}
	}

	public int GetManagerIndex(IEntity entity) => managers.IndexOf(entity);

	public IEnumerator<IEntity> GetEnumerator() => managers.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Set
	public bool SetManagerIndex(IEntity entity, int index)
	{
		int previous = managers.IndexOf(entity);
		if(!managers.SetIndex(entity, index))
			return false;
		if(previous != index)
			ManagersChanged?.Invoke(this);
		return true;
	}

	public bool SwapManagers(IEntity entity1, IEntity entity2)
	{
		if(entity1 == null)
			return false;
		if(entity2 == null)
			return false;
		int index1 = managers.IndexOf(entity1);
		int index2 = managers.IndexOf(entity2);
		return SwapManagers(index1, index2);
	}

	public bool SwapManagers(int index1, int index2)
	{
		if(!managers.Swap(index1, index2))
			return false;
		if(index1 != index2)
			ManagersChanged?.Invoke(this);
		return true;
	}
	#endregion

	#region Add
	public IEntity AddManager<TType>(IEntity entity, int? index = null)
		where TType : class, IComponent => AddManager(entity, typeof(TType), index);

	public IEntity AddManager(IEntity entity, int index) => AddManager(entity, null, index);

	public IEntity AddManager(IEntity entity, Type type = null, int? index = null)
	{
		type = AtlasComponent.GetType(this, type);
		if(entity.GetComponent(type) == this)
		{
			index ??= managers.Count;
			if(!HasManager(entity))
			{
				managers.Insert(index.Value, entity);
				AddingManager(entity, index.Value);
				ManagerAdded?.Invoke(this, entity);
			}
			else
			{
				SetManagerIndex(entity, index.Value);
			}
		}
		else
		{
			entity.AddComponent(this, type, index);
		}
		return entity;
	}

	/// <summary>
	/// Called when an Entity has been added to this Component.
	/// This is called after the add has occured and before any
	/// Signals are dispatched.
	/// </summary>
	/// <param name="entity">The Entity that has been added.</param>
	/// <param name="index">The current index of the Entity being added.</param>
	protected virtual void AddingManager(IEntity entity, int index) { }
	#endregion

	#region Remove
	public IEntity RemoveManager<TType>(IEntity entity)
		where TType : class, IComponent => RemoveManager(entity, typeof(TType));

	public IEntity RemoveManager(IEntity entity) => RemoveManager(entity, entity.GetComponentType(this));

	public IEntity RemoveManager(IEntity entity, Type type = null)
	{
		if(!managers.Contains(entity))
			return null;
		type = AtlasComponent.GetType(this, type);
		if(entity.GetComponent(type) != this)
		{
			int index = managers.IndexOf(entity);
			managers.RemoveAt(index);
			RemovingManager(entity, index);
			ManagerRemoved?.Invoke(this, entity);
			AutoDispose.TryAutoDispose();
		}
		else
		{
			entity.RemoveComponent<IComponent>(type);
		}
		return entity;
	}

	public IEntity RemoveManager(int index) => RemoveManager(managers[index]);

	/// <summary>
	/// Called when an Entity has been removed from this Component.
	/// This is called after the remove has occured and before any
	/// Signals are dispatched.
	/// </summary>
	/// <param name="entity">The Entity that has been removed.</param>
	/// <param name="index">The previous index of the Entity being removed.</param>
	protected virtual void RemovingManager(IEntity entity, int index) { }

	public bool RemoveManagers()
	{
		if(managers.Count <= 0)
			return false;
		while(managers.Count > 0)
			RemoveManager(managers[managers.Count - 1]);
		return true;
	}
	#endregion
	#endregion
}