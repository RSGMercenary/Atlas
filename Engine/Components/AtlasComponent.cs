using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Messages;
using System;
using System.Text;

namespace Atlas.Engine.Components
{
	public enum ComponentEntityRelationship
	{
		OneToOne = 1,
		ManyToOne = 2,
		OneToMany = 4,
		ManyToMany = 8,
	}

	//TO-DO Not sure if this is right. Might be AtlasComponent<IComponent<T>>?
	public abstract class AtlasComponent : AtlasComponent<AtlasComponent>
	{

	}

	public abstract class AtlasComponent<T> : AutoEngineObject<T>, IComponent<T>
		where T : class, IComponent<T>
	{
		private readonly bool isShareable = false;
		private LinkList<IEntity> managers = new LinkList<IEntity>();

		public AtlasComponent() : this(false)
		{

		}

		public AtlasComponent(bool isShareable)
		{
			this.isShareable = isShareable;
		}

		protected override void Destroying()
		{
			Reset();
			base.Destroying();
		}

		public void Reset()
		{
			Resetting();
		}

		protected virtual void Resetting()
		{
			RemoveManagers();
			AutoDestroy = true;
		}

		public bool IsShareable
		{
			get { return isShareable; }
		}

		public IEntity Manager
		{
			get { return !isShareable ? managers.First?.Value : null; }
		}

		public IReadOnlyLinkList<IEntity> Managers
		{
			get { return managers; }
		}

		public int GetManagerIndex(IEntity entity)
		{
			return managers.GetIndex(entity);
		}

		public bool SetManagerIndex(IEntity entity, int index)
		{
			if(!managers.SetIndex(entity, index))
				return false;
			Message(new Message<T>(AtlasMessage.Managers));
			return true;
		}

		public bool SwapManagers(IEntity entity1, IEntity entity2)
		{
			if(entity1 == null)
				return false;
			if(entity2 == null)
				return false;
			int index1 = managers.GetIndex(entity1);
			int index2 = managers.GetIndex(entity2);
			return SwapManagers(index1, index2);
		}

		public bool SwapManagers(int index1, int index2)
		{
			if(!managers.Swap(index1, index2))
				return false;
			Message(new Message<T>(AtlasMessage.Managers));
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
				else if(type == typeof(IComponent<T>))
					return null;
				else if(!type.IsInstanceOfType(this))
					return null;
				if(entity.GetComponent(type) == this)
				{
					//Component is no longer considered destroyed if it's adding Entities.
					Construct();
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Add(entity, index);
					entity.AddListener(AtlasMessage.Engine, EntityEngineChanged);
					Engine = entity.Engine;
					AddingManager(entity, index);
					Message(new KeyValueMessage<T, int, IEntity>(AtlasMessage.AddManager, index, entity));
					Message(new Message<T>(AtlasMessage.Managers));
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
				int index = managers.GetIndex(entity);
				managers.Remove(index);
				entity.RemoveListener(AtlasMessage.Engine, EntityEngineChanged);
				if(managers.IsEmpty)
					Engine = null;
				RemovingManager(entity, index);
				Message(new KeyValueMessage<T, int, IEntity>(AtlasMessage.RemoveManager, index, entity));
				Message(new Message<T>(AtlasMessage.Managers));
				if(AutoDestroy && managers.IsEmpty)
					Destroy();
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
			if(managers.IsEmpty)
				return false;
			while(!managers.IsEmpty)
				RemoveManager(managers.Last.Value);
			return true;
		}

		private void EntityEngineChanged(IMessage<IEntity> message)
		{
			if(!message.AtTarget)
				return;
			Engine = message.Target.Engine;
		}

		sealed override public IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(managers.IsEmpty)
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

		protected override void Messaging(IMessage<T> message)
		{
			if(message.Type == AtlasMessage.AutoDestroy)
			{
				var cast = message as IPropertyMessage<IComponent, bool>;
				if(cast.Current && managers.IsEmpty)
					Destroy();
			}
			base.Messaging(message);
		}

		public string ToInfoString(bool addEntities, int index = 0, string indent = "")
		{
			StringBuilder text = new StringBuilder();
			text.Append(indent + "Component");
			if(index > 0)
				text.Append(" " + index);
			text.AppendLine();
			text.AppendLine(indent + "  Instance    = " + GetType().FullName);
			if(!IsShareable && Manager != null)
				text.AppendLine(indent + "  Interface   = " + Manager.GetComponentType(this).FullName);
			text.AppendLine(indent + "  " + nameof(AutoDestroy) + " = " + AutoDestroy);
			text.AppendLine(indent + "  " + nameof(IsShareable) + " = " + IsShareable);
			if(IsShareable)
			{
				text.AppendLine(indent + "  Entities (" + managers.Count + ")");
				if(addEntities)
				{
					index = 0;
					foreach(IEntity entity in managers)
					{
						text.AppendLine(indent + "    Entity " + (++index));
						text.AppendLine(indent + "      Interface  = " + entity.GetComponentType(this).FullName);
						text.AppendLine(indent + "      " + nameof(entity.GlobalName) + " = " + entity.GlobalName);
					}
				}
			}
			return text.ToString();
		}
	}
}