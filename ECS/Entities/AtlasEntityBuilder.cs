using Atlas.Core.Collections.Builder;
using Atlas.Core.Messages;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities;

public class AtlasEntityBuilder : Builder<IEntityBuilder, IEntity>, IEntityBuilder
{
	public AtlasEntityBuilder() { }
	public AtlasEntityBuilder(IEntity entity) : base(entity) { }

	protected override IEntity NewInstance() => AtlasEntity.Get();

	#region Names
	public IEntityBuilder SetNames(string name)
	{
		Instance.GlobalName = name;
		Instance.LocalName = name;
		return this;
	}

	public IEntityBuilder SetGlobalName(string globalName)
	{
		Instance.GlobalName = globalName;
		return this;
	}

	public IEntityBuilder SetLocalName(string localName)
	{
		Instance.LocalName = localName;
		return this;
	}
	#endregion

	#region Components
	#region Add
	#region KeyValue
	public IEntityBuilder AddComponent<TKeyValue>()
		where TKeyValue : class, IComponent, new()
	{
		Instance.AddComponent<TKeyValue>();
		return this;
	}

	public IEntityBuilder AddComponent<TKeyValue>(TKeyValue component)
		where TKeyValue : class, IComponent
	{
		Instance.AddComponent(component);
		return this;
	}

	public IEntityBuilder AddComponent<TKeyValue>(TKeyValue component, int index)
		where TKeyValue : class, IComponent
	{
		Instance.AddComponent(component, index);
		return this;
	}
	#endregion

	#region Key, Value
	public IEntityBuilder AddComponent<TKey, TValue>()
		where TKey : IComponent
		where TValue : class, TKey, new()
	{
		Instance.AddComponent<TKey, TValue>();
		return this;
	}

	public IEntityBuilder AddComponent<TKey, TValue>(TValue component)
		where TKey : IComponent
		where TValue : class, TKey
	{
		Instance.AddComponent<TKey, TValue>(component);
		return this;
	}

	public IEntityBuilder AddComponent<TKey, TValue>(TValue component, int index)
		where TKey : IComponent
		where TValue : class, TKey
	{
		Instance.AddComponent<TKey, TValue>(component, index);
		return this;
	}
	#endregion

	#region Type, Value
	public IEntityBuilder AddComponent<TValue>(Type type)
		where TValue : class, IComponent, new()
	{
		Instance.AddComponent<TValue>(type);
		return this;
	}

	public IEntityBuilder AddComponent<TValue>(TValue component, Type type)
		where TValue : class, IComponent
	{
		Instance.AddComponent(component, type);
		return this;
	}

	public IEntityBuilder AddComponent<TValue>(TValue component, Type type, int index)
		where TValue : class, IComponent
	{
		Instance.AddComponent(component, type, index);
		return this;
	}
	#endregion

	#region Type, Component
	public IEntityBuilder AddComponent(IComponent component)
	{
		Instance.AddComponent(component);
		return this;
	}

	public IEntityBuilder AddComponent(IComponent component, Type type)
	{
		Instance.AddComponent(component, type);
		return this;
	}

	public IEntityBuilder AddComponent(IComponent component, int index)
	{
		Instance.AddComponent(component, index);
		return this;
	}

	public IEntityBuilder AddComponent(IComponent component, Type type, int index)
	{
		Instance.AddComponent(component, type, index);
		return this;
	}
	#endregion
	#endregion

	#region Remove
	public IEntityBuilder RemoveComponent<TKey, TValue>()
		where TKey : IComponent
		where TValue : class, TKey
	{
		Instance.RemoveComponent<TKey, TValue>();
		return this;
	}

	public IEntityBuilder RemoveComponent<TKeyValue>()
		where TKeyValue : class, IComponent
	{
		Instance.RemoveComponent<TKeyValue>();
		return this;
	}

	public IEntityBuilder RemoveComponent(Type type)
	{
		Instance.RemoveComponent(type);
		return this;
	}

	public IEntityBuilder RemoveComponent(IComponent component)
	{
		Instance.RemoveComponent(component);
		return this;
	}

	public IEntityBuilder RemoveComponents()
	{
		Instance.RemoveComponents();
		return this;
	}
	#endregion
	#endregion

	#region Hierarchy
	#region Root
	public IEntityBuilder SetRoot(bool root)
	{
		Instance.IsRoot = root;
		return this;
	}
	#endregion

	#region Add
	public IEntityBuilder AddChild(IEntity child)
	{
		Instance.AddChild(child);
		return this;
	}

	public IEntityBuilder AddChild(IEntity child, int index)
	{
		Instance.AddChild(child, index);
		return this;
	}
	#endregion

	#region Remove
	public IEntityBuilder RemoveChild(IEntity child)
	{
		Instance.RemoveChild(child);
		return this;
	}

	public IEntityBuilder RemoveChild(int index)
	{
		Instance.RemoveChild(index);
		return this;
	}
	#endregion

	#region Parent
	public IEntityBuilder SetParent(IEntity parent)
	{
		Instance.SetParent(parent);
		return this;
	}

	public IEntityBuilder SetParent(IEntity parent, int index)
	{
		Instance.SetParent(parent, index);
		return this;
	}
	#endregion
	#endregion

	#region Sleeping
	public IEntityBuilder SetSleeping(bool sleeping)
	{
		Instance.IsSleeping = sleeping;
		return this;
	}

	public IEntityBuilder SetSelfSleeping(bool selfSleeping)
	{
		Instance.IsSelfSleeping = selfSleeping;
		return this;
	}
	#endregion

	#region AutoDispose
	public IEntityBuilder SetIsAutoDisposable(bool isAutoDisposable)
	{
		Instance.IsAutoDisposable = isAutoDisposable;
		return this;
	}
	#endregion

	#region Messages
	public IEntityBuilder AddListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<IEntity>
	{
		Instance.AddListener(listener);
		return this;
	}

	public IEntityBuilder AddListener<TMessage>(Action<TMessage> listener, int priority)
		where TMessage : IMessage<IEntity>
	{
		Instance.AddListener(listener, priority);
		return this;
	}

	public IEntityBuilder RemoveListener<TMessage>(Action<TMessage> listener)
		where TMessage : IMessage<IEntity>
	{
		Instance.RemoveListener(listener);
		return this;
	}

	public IEntityBuilder RemoveListeners()
	{
		Instance.RemoveListeners();
		return this;
	}
	#endregion
}