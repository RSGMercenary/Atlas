using Atlas.Core.Objects.Builder;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities
{
	public interface IEntityBuilder : IHierarchyBuilder<IEntityBuilder, IEntity>, IMessengerBuilder<IEntityBuilder, IEntity>
	{
		#region Names
		IEntityBuilder SetNames(string name);

		IEntityBuilder SetGlobalName(string globalName);

		IEntityBuilder SetLocalName(string localName);
		#endregion

		#region Components
		#region KeyValue
		IEntityBuilder AddComponent<TKeyValue>()
			where TKeyValue : class, IComponent, new();

		IEntityBuilder AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : class, IComponent;

		IEntityBuilder AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : class, IComponent;
		#endregion

		#region Key, Value
		IEntityBuilder AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey, new();

		IEntityBuilder AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : class, TKey;

		IEntityBuilder AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : class, TKey;
		#endregion

		#region Type, Value
		IEntityBuilder AddComponent<TValue>(Type type)
			where TValue : class, IComponent, new();

		IEntityBuilder AddComponent<TValue>(TValue component, Type type)
			where TValue : class, IComponent;

		IEntityBuilder AddComponent<TValue>(TValue component, Type type, int index)
			where TValue : class, IComponent;
		#endregion

		#region Type, Component
		IEntityBuilder AddComponent(IComponent component);

		IEntityBuilder AddComponent(IComponent component, Type type);

		IEntityBuilder AddComponent(IComponent component, int index);

		IEntityBuilder AddComponent(IComponent component, Type type, int index);
		#endregion

		#region Remove
		IEntityBuilder RemoveComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey;

		IEntityBuilder RemoveComponent<TKeyValue>()
			where TKeyValue : class, IComponent;

		IEntityBuilder RemoveComponent(Type type);

		IEntityBuilder RemoveComponent(IComponent component);

		IEntityBuilder RemoveComponents();
		#endregion
		#endregion

		#region Sleeping
		IEntityBuilder SetSleeping(bool sleeping);

		IEntityBuilder SetFreeSleeping(bool freeSleeping);
		#endregion

		#region AutoDispose
		IEntityBuilder SetIsAutoDisposable(bool isAutoDisposable);
		#endregion
	}
}