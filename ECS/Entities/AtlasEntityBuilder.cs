using Atlas.Core.Collections.Builder;
using Atlas.ECS.Components.Component;
using System;

namespace Atlas.ECS.Entities
{
	public class AtlasEntityBuilder : Builder<IEntityBuilder, IEntity>, IEntityBuilder
	{
		public AtlasEntityBuilder() { }
		public AtlasEntityBuilder(IEntity entity) : base(entity) { }

		protected override IEntity NewInstance() => AtlasEntity.Get();


		#region Names
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

		#region Hierarchy
		public IEntityBuilder SetRoot(bool root)
		{
			Instance.IsRoot = root;
			return this;
		}

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

		#region Sleeping
		public IEntityBuilder SetSleeping(bool sleeping)
		{
			Instance.IsSleeping = sleeping;
			return this;
		}

		public IEntityBuilder SetFreeSleeping(bool freeSleeping)
		{
			Instance.IsFreeSleeping = freeSleeping;
			return this;
		}
		#endregion

		#region AutoDispose
		public IEntityBuilder SetAutoDispose(bool autoDispose)
		{
			Instance.AutoDispose = autoDispose;
			return this;
		}
		#endregion
	}
}