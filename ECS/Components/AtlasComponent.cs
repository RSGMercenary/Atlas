using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Messages;
using Atlas.ECS.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Components
{
	public abstract class AtlasComponent : AtlasComponent<IComponent>
	{

	}

	public abstract class AtlasComponent<T> : AtlasObject<T>, IComponent<T>
		where T : class, IComponent
	{
		#region Static

		private static Dictionary<Type, IPool> pools = new Dictionary<Type, IPool>();

		public static IReadOnlyPool<TComponent> Pool<TComponent>() where TComponent : AtlasComponent<T>, new()
		{
			var type = typeof(TComponent);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<TComponent>(() => new TComponent(), component => component.Compose()));
			return pools[type] as IReadOnlyPool<TComponent>;
		}

		public static TComponent Get<TComponent>() where TComponent : AtlasComponent<T>, new()
		{
			return Pool<TComponent>().Remove();
		}

		#endregion

		private readonly Group<IEntity> managers = new Group<IEntity>();
		private bool autoDestroy = true;
		public bool IsShareable { get; } = false;

		public AtlasComponent() : this(false)
		{

		}

		public AtlasComponent(bool isShareable)
		{
			IsShareable = isShareable;
		}

		protected override void Disposing(bool finalizer)
		{
			RemoveManagers();
			AutoDestroy = true;

			//AtlasComponent derived class had Dispose() called
			//manually or by ~EngineObject() finalizer.
			//Pool reference for later reuse.
			if(pools.ContainsKey(GetType()))
				pools[GetType()].Add(this);

			base.Disposing(finalizer);
		}

		public bool AutoDestroy
		{
			get { return autoDestroy; }
			set
			{
				if(autoDestroy == value)
					return;
				var previous = autoDestroy;
				autoDestroy = value;
				Dispatch<IAutoDestroyMessage<T>>(new AutoDestroyMessage<T>(this as T, value, previous));
				if(autoDestroy && managers.Count <= 0)
					Dispose();
			}
		}

		public IEntity Manager
		{
			get { return !IsShareable && managers.Count > 0 ? managers[0] : null; }
		}

		public IReadOnlyGroup<IEntity> Managers
		{
			get { return managers; }
		}

		public bool HasManager(IEntity entity)
		{
			return managers.Contains(entity);
		}

		public int GetManagerIndex(IEntity entity)
		{
			return managers.IndexOf(entity);
		}

		public bool SetManagerIndex(IEntity entity, int index)
		{
			if(!managers.SetIndex(entity, index))
				return false;
			Dispatch<IManagerMessage>(new ManagerMessage(this));
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
			Dispatch<IManagerMessage>(new ManagerMessage(this));
			return true;
		}

		public IEntity AddManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent
		{
			return AddManager(entity, typeof(TIComponent), int.MaxValue);
		}

		public IEntity AddManager<TIComponent>(IEntity entity, int index)
			where TIComponent : IComponent
		{
			return AddManager(entity, typeof(TIComponent), index);
		}

		public IEntity AddManager(IEntity entity)
		{
			return AddManager(entity, null);
		}

		public IEntity AddManager(IEntity entity, Type type)
		{
			return AddManager(entity, type, int.MaxValue);
		}

		public IEntity AddManager(IEntity entity, int index)
		{
			return AddManager(entity, null, index);
		}

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
					entity.AddListener<IEngineMessage<IEntity>>(EntityEngineChanged);
					Engine = entity.Engine;
					AddingManager(entity, index);
					Dispatch<IManagerAddMessage>(new ManagerAddMessage(this, index, entity));
					Dispatch<IManagerMessage>(new ManagerMessage(this));
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
		protected virtual void AddingManager(IEntity entity, int index)
		{

		}

		public IEntity RemoveManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent
		{
			return RemoveManager(entity, typeof(TIComponent));
		}

		public IEntity RemoveManager(IEntity entity)
		{
			return RemoveManager(entity, entity?.GetComponentType(this));
		}

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
				entity.RemoveListener<IEngineMessage<IEntity>>(EntityEngineChanged);
				if(managers.Count <= 0)
					Engine = null;
				RemovingManager(entity, index);
				Dispatch<IManagerRemoveMessage>(new ManagerRemoveMessage(this, index, entity));
				Dispatch<IManagerMessage>(new ManagerMessage(this));
				if(autoDestroy && managers.Count <= 0)
					Dispose();
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
		protected virtual void RemovingManager(IEntity entity, int index)
		{

		}

		public bool RemoveManagers()
		{
			if(managers.Count <= 0)
				return false;
			while(managers.Count > 0)
				RemoveManager(managers[managers.Count - 1]);
			return true;
		}

		private void EntityEngineChanged(IEngineMessage<IEntity> message)
		{
			Engine = message.CurrentValue;
		}

		public sealed override IEngine Engine
		{
			get { return base.Engine; }
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

		public string ToInfoString(bool addEntities, int index = 0, string indent = "")
		{
			var info = new StringBuilder();
			info.Append($"{indent}Component");
			if(index > 0)
				info.Append($" {index}");
			info.AppendLine();
			info.AppendLine($"{indent}  Instance    = {GetType().FullName}");
			if(Manager != null)
				info.AppendLine($"{indent}  Interface   = {Manager.GetComponentType(this).FullName}");
			info.AppendLine($"{indent}  {nameof(AutoDestroy)} = {AutoDestroy}");
			info.AppendLine($"{indent}  {nameof(IsShareable)} = {IsShareable}");
			if(IsShareable)
			{
				info.AppendLine($"{indent}  Entities ({managers.Count})");
				if(addEntities)
				{
					index = 0;
					foreach(var entity in managers)
					{
						info.AppendLine($"{indent}    Entity {++index}");
						info.AppendLine($"{indent}      Interface  = {entity.GetComponentType(this).FullName}");
						info.AppendLine($"{indent}      {nameof(entity.GlobalName)} = {entity.GlobalName}");
					}
				}
			}
			return info.ToString();
		}
	}
}