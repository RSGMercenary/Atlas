using Atlas.Core.Objects.Builder;
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
	#region Component & Type
	public IEntityBuilder AddComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new()
	{
		Instance.AddComponent<TComponent, TType>();
		return this;
	}

	public IEntityBuilder AddComponent<TComponent, TType>(TComponent component)
		where TType : class, IComponent
		where TComponent : class, TType
	{
		Instance.AddComponent<TComponent, TType>(component);
		return this;
	}

	public IEntityBuilder AddComponent<TComponent, TType>(TComponent component, int index)
		where TType : class, IComponent
		where TComponent : class, TType
	{
		Instance.AddComponent<TComponent, TType>(component, index);
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
	public IEntityBuilder RemoveComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType
	{
		Instance.RemoveComponent<TComponent, TType>();
		return this;
	}

	public IEntityBuilder RemoveComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent
	{
		Instance.RemoveComponent<TComponent>(type);
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
		Instance.Sleep(sleeping);
		return this;
	}

	public IEntityBuilder SetSelfSleeping(bool selfSleeping)
	{
		Instance.SelfSleep(selfSleeping);
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
}