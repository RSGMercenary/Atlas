using Atlas.Core.Objects.Builder;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities;

public interface IEntityBuilder : IHierarchyBuilder<IEntityBuilder, IEntity>, IBuilder<IEntityBuilder, IEntity>
{
	#region Names
	IEntityBuilder SetNames(string name);

	IEntityBuilder SetGlobalName(string globalName);

	IEntityBuilder SetLocalName(string localName);
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

	#region Sleeping
	IEntityBuilder SetSleeping(bool sleeping);

	IEntityBuilder SetSelfSleeping(bool selfSleeping);
	#endregion

	#region AutoDispose
	IEntityBuilder SetIsAutoDisposable(bool isAutoDisposable);
	#endregion
}