using Atlas.Core.Objects.Builder;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities;

public interface IEntityBuilder : IBuilder<IEntityBuilder, IEntity>
{
	#region Names
	IEntityBuilder SetNames(string name);

	IEntityBuilder GlobalName(string globalName);

	IEntityBuilder LocalName(string localName);
	#endregion

	#region Components
	#region Add
	#region Component & Type
	IEntityBuilder AddComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new();

	IEntityBuilder AddComponent<TComponent, TType>(TComponent component, int index)
		where TType : class, IComponent
		where TComponent : class, TType;

	IEntityBuilder AddComponent<TComponent, TType>(TComponent component)
		where TType : class, IComponent
		where TComponent : class, TType;
	#endregion

	#region Type
	IEntityBuilder AddComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent, new();

	IEntityBuilder AddComponent<TComponent>(TComponent component, int index)
		where TComponent : class, IComponent;

	IEntityBuilder AddComponent<TComponent>(TComponent component, Type type = null, int? index = null)
		where TComponent : class, IComponent;
	#endregion
	#endregion

	#region Remove
	IEntityBuilder RemoveComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType;

	IEntityBuilder RemoveComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent;

	IEntityBuilder RemoveComponent(IComponent component);

	IEntityBuilder RemoveComponents();
	#endregion
	#endregion

	#region Hierarchy
	IEntityBuilder IsRoot(bool root);

	IEntityBuilder AddChild(IEntity child);

	IEntityBuilder AddChild(IEntity child, int index);

	IEntityBuilder RemoveChild(IEntity child);

	IEntityBuilder RemoveChild(int index);

	IEntityBuilder SetParent(IEntity parent);

	IEntityBuilder SetParent(IEntity parent, int index);
	#endregion

	#region Sleeping
	IEntityBuilder Sleep(bool sleeping);

	IEntityBuilder SelfSleep(bool selfSleeping);
	#endregion

	#region AutoDispose
	IEntityBuilder AutoDispose(bool isAutoDisposable);
	#endregion
}