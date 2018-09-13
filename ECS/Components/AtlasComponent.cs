using Atlas.ECS.Entities;
using Atlas.ECS.Objects;
using Atlas.Framework.Collections.EngineList;
using Atlas.Framework.Collections.Pool;
using Atlas.Framework.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Components
{
	public abstract class AtlasComponent : EngineObject, IComponent
	{
		#region Static

		private static Dictionary<Type, IPool> pools = new Dictionary<Type, IPool>();

		public static IReadOnlyPool<TComponent> Pool<TComponent>() where TComponent : AtlasComponent, new()
		{
			var type = typeof(TComponent);
			if(!pools.ContainsKey(type))
				pools.Add(type, new Pool<TComponent>(() => new TComponent(), component => component.Initialize()));
			return pools[type] as IReadOnlyPool<TComponent>;
		}

		public static TComponent Get<TComponent>() where TComponent : AtlasComponent, new()
		{
			return Pool<TComponent>().Remove();
		}

		#endregion

		private EngineList<IEntity> managers = new EngineList<IEntity>();
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
				Message<IAutoDestroyMessage>(new AutoDestroyMessage(this, value, previous));
			}
		}

		public IEntity Manager
		{
			get { return !IsShareable && managers.Count > 0 ? managers[0] : null; }
		}

		public IReadOnlyEngineList<IEntity> Managers
		{
			get { return managers; }
		}

		public int GetManagerIndex(IEntity entity)
		{
			return managers.IndexOf(entity);
		}

		public bool SetManagerIndex(IEntity entity, int index)
		{
			if(!managers.SetIndex(entity, index))
				return false;
			Message<IManagerMessage>(new ManagerMessage(this));
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
			Message<IManagerMessage>(new ManagerMessage(this));
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
			if(entity == null)
				return null;
			if(!managers.Contains(entity))
			{
				if(type == null)
					type = GetType();
				else if(type == typeof(IComponent))
					return null;
				else if(!type.IsInstanceOfType(this))
					return null;
				if(entity.GetComponent(type) == this)
				{
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Insert(index, entity);
					entity.AddListener<IEngineMessage>(EntityEngineChanged);
					Engine = entity.Engine;
					AddingManager(entity, index);
					Message<IManagerAddMessage>(new ManagerAddMessage(this, index, entity));
					Message<IManagerMessage>(new ManagerMessage(this));
				}
				else
				{
					if(entity.AddComponent(this, type, index) == null)
						return null;
				}
			}
			else
			{
				SetManagerIndex(entity, index);
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
				entity.RemoveListener<IEngineMessage>(EntityEngineChanged);
				if(managers.Count <= 0)
					Engine = null;
				RemovingManager(entity, index);
				Message<IManagerRemoveMessage>(new ManagerRemoveMessage(this, index, entity));
				Message<IManagerMessage>(new ManagerMessage(this));
				if(AutoDestroy && managers.Count <= 0)
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

		private void EntityEngineChanged(IEngineMessage message)
		{
			Engine = message.CurrentValue;
		}

		public sealed override IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(managers.Count <= 0)
				{
					base.Engine = null;
				}
				else
				{
					int same = 0;
					foreach(var manager in managers)
					{
						if(manager.Engine == value)
							++same;
					}
					if(managers.Count == same)
						base.Engine = value;
					else if(same > 0)
						base.Engine = null;
				}
			}
		}

		protected override void Messaging(IMessage message)
		{
			if(message is IAutoDestroyMessage)
			{
				var cast = message as IAutoDestroyMessage;
				if(cast.CurrentValue && managers.Count <= 0)
					Dispose();
			}
			base.Messaging(message);
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