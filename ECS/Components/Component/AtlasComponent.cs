using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.ECS.Components.Messages;
using Atlas.ECS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Components
{
	public abstract class AtlasComponent<T> : Messenger<T>, IComponent<T>
		where T : class, IComponent
	{
		#region Static

		private static readonly Dictionary<Type, IPool> pools = new Dictionary<Type, IPool>();

		public static IReadOnlyPool<TComponent> Pool<TComponent>()
			where TComponent : class, IComponent, new()
		{
			var type = typeof(TComponent);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<TComponent>());
			return pools[type] as IReadOnlyPool<TComponent>;
		}

		public static TComponent Get<TComponent>()
			where TComponent : class, IComponent, new()
		{
			return Pool<TComponent>().Remove();
		}

		#endregion

		#region Fields

		private readonly Group<IEntity> managers = new Group<IEntity>();
		private bool autoDispose = true;
		public bool IsShareable { get; } = false;

		#endregion

		#region Construct / Dispose

		public AtlasComponent() : this(false) { }

		public AtlasComponent(bool isShareable)
		{
			IsShareable = isShareable;
		}

		protected override void Disposing()
		{
			RemoveManagers();
			AutoDispose = true;

			//AtlasComponent derived class had Dispose() called
			//manually. Pool reference for later reuse.
			if(pools.ContainsKey(GetType()))
				pools[GetType()].Add(this);

			base.Disposing();
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

		public IEntity Manager => !IsShareable && managers.Count > 0 ? managers[0] : null;

		public IReadOnlyGroup<IEntity> Managers => managers;

		public bool HasManager(IEntity entity) => managers.Contains(entity);

		public int GetManagerIndex(IEntity entity) => managers.IndexOf(entity);

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

		public IEntity AddManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent => AddManager(entity, typeof(TIComponent), int.MaxValue);

		public IEntity AddManager<TIComponent>(IEntity entity, int index)
			where TIComponent : IComponent => AddManager(entity, typeof(TIComponent), index);

		public IEntity AddManager(IEntity entity) => AddManager(entity, null);

		public IEntity AddManager(IEntity entity, Type type) => AddManager(entity, type, int.MaxValue);

		public IEntity AddManager(IEntity entity, int index) => AddManager(entity, null, index);

		public IEntity AddManager(IEntity entity, Type type, int index)
		{
			type = type ?? GetType();
			if(!type.IsInstanceOfType(this))
				return null;
			if(entity?.GetComponent(type) == this)
			{
				if(!HasManager(entity))
				{
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Insert(index, entity);
					/*
					entity.AddListener<IEngineMessage<IEntity>>(EntityEngineChanged);
					Engine = entity.Engine;
					*/
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

		public IEntity RemoveManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent => RemoveManager(entity, typeof(TIComponent));

		public IEntity RemoveManager(IEntity entity) => RemoveManager(entity, entity?.GetComponentType(this));

		public IEntity RemoveManager(IEntity entity, Type type)
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
				/*
				entity.RemoveListener<IEngineMessage<IEntity>>(EntityEngineChanged);
				if(managers.Count <= 0)
					Engine = null;
				*/
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

		#region Engine
		/*
		private void EntityEngineChanged(IEngineMessage<IEntity> message)
		{
			Engine = message.CurrentValue;
		}

		public sealed override IEngine Engine
		{
			get => base.Engine;
			set
			{
				int count = 0;
				foreach(var manager in managers)
				{
					if(manager.Engine == value)
						++count;
				}
				if(count <= 0)
					base.Engine = null;
				else if(managers.Count == count)
					base.Engine = value;
			}
		}
		*/
		#endregion

		#region Info Strings

		public string ToInfoString(bool addEntities, int index = 0, string indent = "", StringBuilder text = null)
		{
			text = text ?? new StringBuilder();
			text.Append($"{indent}Component");
			if(index > 0)
				text.Append($" {index}");
			text.AppendLine();
			text.AppendLine($"{indent}  Instance    = {GetType().FullName}");
			if(Manager != null)
				text.AppendLine($"{indent}  Interface   = {Manager.GetComponentType(this).FullName}");
			text.AppendLine($"{indent}  {nameof(AutoDispose)} = {AutoDispose}");
			text.AppendLine($"{indent}  {nameof(IsShareable)} = {IsShareable}");
			if(IsShareable)
			{
				text.AppendLine($"{indent}  Entities ({managers.Count})");
				if(addEntities)
				{
					index = 0;
					foreach(var entity in managers)
					{
						text.AppendLine($"{indent}    Entity {++index}");
						text.AppendLine($"{indent}      Interface  = {entity.GetComponentType(this).FullName}");
						text.AppendLine($"{indent}      {nameof(entity.GlobalName)} = {entity.GlobalName}");
					}
				}
			}
			return text.ToString();
		}

		#endregion
	}
}