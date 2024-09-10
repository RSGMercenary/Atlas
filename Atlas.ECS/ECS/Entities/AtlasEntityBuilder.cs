using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities;

public class AtlasEntityBuilder : IEntityBuilder
{
	private IEntity Instance { get; set; }

	public AtlasEntityBuilder() { }

	public AtlasEntityBuilder(IEntity entity) => Instance = entity;

	public IEntityBuilder Start()
	{
		Instance ??= AtlasEntity.Get();
		return this;
	}

	public IEntity Finish()
	{
		var instance = Instance;
		Instance = null;
		return instance;
	}

	#region Names
	public IEntityBuilder SetNames(string name)
	{
		Instance.GlobalName = name;
		Instance.LocalName = name;
		return this;
	}

	public IEntityBuilder GlobalName(string globalName)
	{
		Instance.GlobalName = globalName;
		return this;
	}

	public IEntityBuilder LocalName(string localName)
	{
		Instance.LocalName = localName;
		return this;
	}
	#endregion

	#region Components
	#region Add
	#region Component & Type
	public IEntityBuilder AddComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		Instance.AddComponent<TComponent, TType>();
		return this;
	}
	#endregion

	#region Component
	public IEntityBuilder AddComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent, new()
	{
		Instance.AddComponent<TComponent>(type);
		return this;
	}

	public IEntityBuilder AddComponent<TComponent>(TComponent component, int index)
		where TComponent : class, IComponent
	{
		Instance.AddComponent(component, index);
		return this;
	}

	public IEntityBuilder AddComponent<TComponent>(TComponent component, Type type = null, int? index = null)
		where TComponent : class, IComponent
	{
		Instance.AddComponent(component, type, index);
		return this;
	}
	#endregion
	#endregion

	#region Remove
	public IEntityBuilder RemoveComponent<TType>()
		where TType : class, IComponent
	{
		Instance.RemoveComponent<TType>();
		return this;
	}

	public IEntityBuilder RemoveComponent(Type type)
	{
		Instance.RemoveComponent(type);
		return this;
	}

	public IEntityBuilder RemoveComponent<TComponent>(TComponent component)
		where TComponent : class, IComponent
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
	public IEntityBuilder IsRoot(bool isRoot)
	{
		Instance.IsRoot = isRoot;
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

	#region Sleep
	public IEntityBuilder Sleep(bool sleep)
	{
		Instance.Sleep(sleep);
		return this;
	}

	public IEntityBuilder SelfSleep(bool selfSleep)
	{
		Instance.SelfSleep(selfSleep);
		return this;
	}
	#endregion

	#region AutoDispose
	public IEntityBuilder AutoDispose(bool autoDispose)
	{
		Instance.AutoDispose = autoDispose;
		return this;
	}
	#endregion
}