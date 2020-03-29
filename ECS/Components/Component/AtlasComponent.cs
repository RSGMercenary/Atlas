﻿using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Component
{
	public abstract class AtlasComponent : AtlasComponent<IComponent>
	{
		#region Static
		#region Pools
		private static readonly Dictionary<Type, IPool> pools = new Dictionary<Type, IPool>();

		public static IReadOnlyPool<TComponent> AddPool<TComponent>()
			where TComponent : class, IComponent, new() => AddPool(() => new TComponent());

		public static IReadOnlyPool<TComponent> AddPool<TComponent>(Func<TComponent> creator)
			where TComponent : class, IComponent
		{
			var type = typeof(TComponent);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<TComponent>(creator));
			return pools[type] as IReadOnlyPool<TComponent>;
		}

		public static bool RemovePool<TComponent>()
			where TComponent : class, IComponent => pools.Remove(typeof(TComponent));

		public static IReadOnlyPool<TComponent> GetPool<TComponent>()
			where TComponent : class, IComponent
		{
			var type = typeof(TComponent);
			return pools.ContainsKey(type) ? pools[type] as IReadOnlyPool<TComponent> : null;
		}
		#endregion

		#region Get/Release
		public static TComponent Get<TComponent>()
			where TComponent : class, IComponent, new() => GetPool<TComponent>()?.Get() ?? new TComponent();

		internal static void Release(IComponent component)
		{
			var type = component.GetType();
			if(pools.ContainsKey(type))
				pools[type].Release(component);
		}
		#endregion
		#endregion
	}

	public abstract class AtlasComponent<T> : Messenger<T>, IComponent<T>
		where T : class, IComponent
	{
		#region Fields
		private readonly Group<IEntity> managers = new Group<IEntity>();
		private bool autoDispose = true;
		public bool IsShareable { get; } = false;
		#endregion

		#region Construct / Dispose

		protected AtlasComponent() : this(false) { }

		protected AtlasComponent(bool isShareable)
		{
			IsShareable = isShareable;
		}

		protected override void Disposing()
		{
			RemoveManagers();
			AutoDispose = true;

			base.Disposing();

			AtlasComponent.Release(this);
		}

		#endregion

		#region AutoDispose
		public bool AutoDispose
		{
			get => autoDispose;
			set
			{
				if(autoDispose == value)
					return;
				var previous = autoDispose;
				autoDispose = value;
				Message<IAutoDisposeMessage<T>>(new AutoDisposeMessage<T>(value, previous));
				TryAutoDispose();
			}
		}

		private void TryAutoDispose()
		{
			if(autoDispose && managers.Count <= 0)
				Dispose();
		}
		#endregion

		#region Managers
		#region Has
		public bool HasManager(IEntity entity) => managers.Contains(entity);
		#endregion

		#region Get
		public IEntity Manager => !IsShareable && managers.Count > 0 ? managers[0] : null;

		public IReadOnlyGroup<IEntity> Managers => managers;

		public int GetManagerIndex(IEntity entity) => managers.IndexOf(entity);
		#endregion

		#region Set
		public bool SetManagerIndex(IEntity entity, int index)
		{
			if(!managers.SetIndex(entity, index))
				return false;
			Message<IManagerMessage<T>>(new ManagerMessage<T>());
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
			Message<IManagerMessage<T>>(new ManagerMessage<T>());
			return true;
		}
		#endregion

		#region Add
		public IEntity AddManager<TKey>(IEntity entity)
			where TKey : IComponent => AddManager(entity, typeof(TKey));

		public IEntity AddManager<TKey>(IEntity entity, int index)
			where TKey : IComponent => AddManager(entity, typeof(TKey), index);

		public IEntity AddManager(IEntity entity) => AddManager(entity, null);

		public IEntity AddManager(IEntity entity, Type type) => AddManager(entity, type, managers.Count);

		public IEntity AddManager(IEntity entity, int index) => AddManager(entity, null, index);

		public virtual IEntity AddManager(IEntity entity, Type type, int index)
		{
			type ??= GetType();
			if(!type.IsInstanceOfType(this))
				return null;
			if(entity?.GetComponent(type) == this)
			{
				if(!HasManager(entity))
				{
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Insert(index, entity);
					AddingManager(entity, index);
					Message<IManagerAddMessage<T>>(new ManagerAddMessage<T>(index, entity));
					Message<IManagerMessage<T>>(new ManagerMessage<T>());
				}
				else
				{
					SetManagerIndex(entity, index);
				}
			}
			else
			{
				if(entity?.AddComponent(this, type, index) == null)
					return null;
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
		public IEntity RemoveManager<TKey>(IEntity entity)
			where TKey : IComponent => RemoveManager(entity, typeof(TKey));

		public IEntity RemoveManager(IEntity entity) => RemoveManager(entity, entity?.GetComponentType(this));

		public virtual IEntity RemoveManager(IEntity entity, Type type)
		{
			if(entity == null)
				return null;
			if(!managers.Contains(entity))
				return null;
			if(type == null)
				type = GetType();
			else if(type == typeof(IComponent))
				return null;
			else if(!type.IsInstanceOfType(this))
				return null;
			if(entity.GetComponent(type) != this)
			{
				int index = managers.IndexOf(entity);
				managers.RemoveAt(index);
				RemovingManager(entity, index);
				Message<IManagerRemoveMessage<T>>(new ManagerRemoveMessage<T>(index, entity));
				Message<IManagerMessage<T>>(new ManagerMessage<T>());
				TryAutoDispose();
			}
			else
			{
				entity.RemoveComponent(type);
			}
			return entity;
		}

		public IEntity RemoveManager(int index)
		{
			if(index < 0)
				return null;
			if(index > managers.Count - 1)
				return null;
			return RemoveManager(managers[index]);
		}

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
}