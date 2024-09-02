using Atlas.Core.Collections.LinkList;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Entities;
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
			throw new ArgumentException($"'{component.GetType()}' is not assignable to '{type}'.");
		return type;
	}
	#endregion
}

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public abstract class AtlasComponent<T> : IComponent<T>
	where T : class, IComponent<T>
{
	#region Events
	public event Action<T, bool> AutoDisposeChanged
	{
		add => AutoDisposer.AutoDisposeChanged += value;
		remove => AutoDisposer.AutoDisposeChanged -= value;
	}

	event Action<IAutoDisposer, bool> IAutoDisposer.AutoDisposeChanged
	{
		add => AutoDisposer.AutoDisposeChanged += value;
		remove => AutoDisposer.AutoDisposeChanged -= value;
	}

	public event Action<T, IEntity> ManagerAdded;

	public event Action<T, IEntity> ManagerRemoved;

	public event Action<T> ManagersChanged;

	event Action<IComponent, IEntity> IComponent.ManagerAdded
	{
		add => ManagerAdded += value;
		remove => ManagerAdded -= value;
	}

	event Action<IComponent, IEntity> IComponent.ManagerRemoved
	{
		add => ManagerRemoved += value;
		remove => ManagerRemoved -= value;
	}

	event Action<IComponent> IComponent.ManagersChanged
	{
		add => ManagersChanged += value;
		remove => ManagersChanged -= value;
	}
	#endregion

	#region Fields
	private readonly LinkList<IEntity> managers = new();
	private readonly AutoDisposer<T> AutoDisposer;
	#endregion

	#region Construct / Dispose
	protected AtlasComponent() : this(false) { }

	protected AtlasComponent(bool isShareable)
	{
		IsShareable = isShareable;
		AutoDisposer = new(this as T, () => managers.Count <= 0);
		AutoDisposer.AutoDispose = Atlas.DefaultComponentAutoDispose;
	}

	public void Dispose()
	{
		Disposing();
	}

	protected virtual void Disposing()
	{
		RemoveManagers();
		AutoDisposer.Dispose();

		PoolManager.Instance.Put(this);
	}
	#endregion

	[JsonProperty(Order = int.MinValue)]
	public bool IsShareable { get; private set; } = false;

	#region AutoDispose
	[JsonProperty(Order = int.MinValue + 1)]
	public bool AutoDispose
	{
		get => AutoDisposer.AutoDispose;
		set => AutoDisposer.AutoDispose = value;
	}
	#endregion

	#region Managers
	#region Has
	public bool HasManager(IEntity entity) => managers.Contains(entity);
	#endregion

	#region Get
	public IEntity Manager => !IsShareable && managers.Count > 0 ? managers[0] : null;

	public IReadOnlyLinkList<IEntity> Managers => managers;

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

	public int GetManagerIndex(IEntity entity) => managers.GetIndex(entity);

	public IEnumerator<IEntity> GetEnumerator() => managers.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	#endregion

	#region Set
	public bool SetManagerIndex(IEntity entity, int index)
	{
		int previous = managers.Set(entity, index);
		if(previous < 0)
			return false;
		if(previous != index)
			ManagersChanged?.Invoke(this as T);
		return true;
	}

	public bool SwapManagers(IEntity entity1, IEntity entity2)
	{
		if(!managers.Swap(entity1, entity2))
			return false;
		if(entity1 != entity2)
			ManagersChanged?.Invoke(this as T);
		return true;
	}

	public bool SwapManagers(int index1, int index2)
	{
		if(!managers.Swap(index1, index2))
			return false;
		if(index1 != index2)
			ManagersChanged?.Invoke(this as T);
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
			if(!HasManager(entity))
			{
				index ??= managers.Count;
				if(SwapNonShareable())
					index = 0;

				managers.Add(entity, index.Value);
				AddingManager(entity, index.Value);
				ManagerAdded?.Invoke(this as T, entity);
			}
			else
			{
				index ??= managers.Count - 1;
				SetManagerIndex(entity, index.Value);
			}
		}
		else
		{
			entity.AddComponent(this, type, index);
		}
		return entity;
	}

	private bool SwapNonShareable()
	{
		if(Manager == null)
			return false;

		var isAutoDisposable = AutoDispose;
		AutoDispose = false;
		RemoveManagers();
		AutoDispose = isAutoDisposable;
		return true;
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
			int index = managers.GetIndex(entity);
			managers.Remove(index);
			RemovingManager(entity, index);
			ManagerRemoved?.Invoke(this as T, entity);
			AutoDisposer.TryAutoDispose();
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
			RemoveManager(managers.Last.Value);
		return true;
	}
	#endregion
	#endregion
}