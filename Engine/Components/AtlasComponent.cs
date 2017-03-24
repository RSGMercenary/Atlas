using Atlas.Engine.Collections;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Text;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent : AutoEngineObject<IComponent>, IComponent
	{
		private readonly bool isShareable = false;
		private LinkList<IEntity> managers = new LinkList<IEntity>();

		private Signal<IComponent, IEntity, int> managerAdded = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, IEntity, int> managerRemoved = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, int, int, CollectionChange> managerIndicesChanged = new Signal<IComponent, int, int, CollectionChange>();

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
			managerAdded.Dispose();
			managerRemoved.Dispose();
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

		protected override void ChangingAutoDestroy(bool current, bool previous)
		{
			base.ChangingAutoDestroy(current, previous);
			if(current && managers.IsEmpty)
				Destroy();
		}

		public bool IsShareable
		{
			get { return isShareable; }
		}

		public ISignal<IComponent, IEntity, int> ManagerAdded
		{
			get { return managerAdded; }
		}

		public ISignal<IComponent, IEntity, int> ManagerRemoved
		{
			get { return managerRemoved; }
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
			return managers.SetIndex(entity, index);
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
			managerIndicesChanged.Dispatch(this, Math.Min(index1, index2), Math.Max(index1, index2), CollectionChange.Swap);
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
					//Component is no longer considered destroyed if it's adding Entities.
					Construct();
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Add(entity, index);
					entity.EngineChanged.Add(EntityEngineChanged, int.MinValue);
					Engine = entity.Engine;
					AddingManager(entity, index);
					managerAdded.Dispatch(this, entity, index);
					managerIndicesChanged.Dispatch(this, index, managers.Count - 1, CollectionChange.Add);
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
			if(entity.GetComponent(type) == null)
			{
				int index = managers.GetIndex(entity);
				managers.Remove(index);
				entity.EngineChanged.Remove(EntityEngineChanged);
				if(managers.IsEmpty)
					Engine = null;
				RemovingManager(entity, index);
				managerRemoved.Dispatch(this, entity, index);
				managerIndicesChanged.Dispatch(this, index, managers.Count - 1, CollectionChange.Remove);
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

		private void EntityEngineChanged(IEntity entity, IEngine next = null, IEngine previous = null)
		{
			Engine = entity.Engine;
		}

		sealed override public IEngine Engine
		{
			get
			{
				return base.Engine;
			}
			set
			{
				if(managers.IsEmpty)
				{
					base.Engine = null;
				}
				else
				{
					int same = 0;
					foreach(var entity in managers)
					{
						if(entity.Engine == value)
							++same;
					}
					if(managers.Count == same)
						base.Engine = value;
					else if(same > 0)
						base.Engine = null;
				}
			}
		}

		public override string ToString()
		{
			return ToString(true);
		}

		/*
		virtual protected void SetToStringProperties(Queue<KeyValuePair<string, string>> properties, string indent = "")
		{
			properties.Enqueue(new KeyValuePair<string, string>("Instance", GetType().FullName));
			properties.Enqueue(new KeyValuePair<string, string>("Shareable", isShareable.ToString()));
			properties.Enqueue(new KeyValuePair<string, string>("Audio Dispose", isAutoDisposed.ToString()));
		}
		*/
		public string ToString(bool addEntities, int index = 0, string indent = "")
		{
			StringBuilder text = new StringBuilder();
			text.Append(indent + "Component");
			if(index > 0)
				text.Append(" " + index);
			text.AppendLine();
			/*Queue<KeyValuePair<string, string>> properties = new Queue<KeyValuePair<string, string>>();
			SetToStringProperties(properties, indent + "  ");
			while(properties.Count > 0)
			{
				KeyValuePair<string, string> property = properties.Dequeue();
				text.AppendLine(indent + "  " + property.Key.PadRight(20, '.') + property.Value);
			}*/
			text.AppendLine(indent + "  Instance     = " + GetType().FullName);
			text.AppendLine(indent + "  Shareable    = " + isShareable);
			text.AppendLine(indent + "  Auto Destroy = " + AutoDestroy);
			text.AppendLine(indent + "  Entities (" + managers.Count + ")");
			if(addEntities)
			{
				index = 0;
				foreach(IEntity entity in managers)
				{
					text.AppendLine(indent + "    Entity " + (++index));
					text.AppendLine(indent + "      Interface  = " + entity.GetComponentType(this).FullName);
					text.AppendLine(indent + "      GlobalName = " + entity.GlobalName);
				}
			}
			return text.ToString();
		}
	}
}