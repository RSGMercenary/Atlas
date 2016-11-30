using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Text;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent:IComponent
	{
		IEngine engine;
		private readonly bool isShareable = false;
		private bool isAutoDisposed = true;
		private bool isDisposing = false;
		private LinkList<IEntity> managers = new LinkList<IEntity>();

		private Signal<IComponent, IEngine, IEngine> engineChanged = new Signal<IComponent, IEngine, IEngine>();
		private Signal<IComponent, IEntity, int> managerAdded = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, IEntity, int> managerRemoved = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent> disposed = new Signal<IComponent>();

		public static implicit operator bool(AtlasComponent component)
		{
			return component != null;
		}

		public AtlasComponent()
		{

		}

		public AtlasComponent(bool isShareable = false)
		{
			this.isShareable = isShareable;
		}

		public void Dispose()
		{
			if(!isDisposing)
			{
				isDisposing = true;
				Disposing();
				isDisposing = false;
			}
		}

		protected virtual void Disposing()
		{
			IsAutoDisposed = true;
			RemoveManagers();
			engineChanged.Dispose();
			managerAdded.Dispose();
			managerRemoved.Dispose();
			disposed.Dispatch(this);
			disposed.Dispose();
		}

		public ISignal<IComponent> Disposed
		{
			get
			{
				return disposed;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return isDisposing;
			}
		}

		public bool IsAutoDisposed
		{
			get
			{
				return isAutoDisposed;
			}
			set
			{
				if(isAutoDisposed == value)
					return;
				isAutoDisposed = value;
				if(managers.IsEmpty && isAutoDisposed)
					Dispose();
			}
		}

		public bool IsShareable
		{
			get
			{
				return isShareable;
			}
		}

		public ISignal<IComponent, IEntity, int> ManagerAdded
		{
			get
			{
				return managerAdded;
			}
		}

		public ISignal<IComponent, IEntity, int> ManagerRemoved
		{
			get
			{
				return managerRemoved;
			}
		}

		public IEntity Manager
		{
			get
			{
				return (!isShareable && !managers.IsEmpty) ? managers.First.Value : null;
			}
		}

		public IReadOnlyLinkList<IEntity> Managers
		{
			get
			{
				return managers;
			}
		}

		public int GetManagerIndex(IEntity entity)
		{
			return managers.GetIndex(entity);
		}

		public bool SetManagerIndex(IEntity entity, int index)
		{
			return managers.SetIndex(entity, index);
		}

		public bool SwapEntities(AtlasEntity entity1, AtlasEntity entity2)
		{
			return managers.Swap(entity1, entity2);
		}

		public bool SwapComponentManagers(int index1, int index2)
		{
			return managers.Swap(index1, index2);
		}

		public IEntity AddManager(IEntity entity)
		{
			return AddManager(entity, null);
		}

		public IEntity AddManager(IEntity entity, Type type)
		{
			return AddManager(entity, type, managers.Count);
		}

		public IEntity AddManager(IEntity entity, int index)
		{
			return AddManager(entity, null, index);
		}

		public IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue)
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
					managers.Add(entity, index);
					entity.EngineChanged.Add(EntityEngineChanged, int.MinValue);
					Engine = entity.Engine;
					AddingManager(entity, index);
					managerAdded.Dispatch(this, entity, index);
				}
				else
				{
					//TO-DO Pass back null if the AddComponent() isn't successful.
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

		protected virtual void AddingManager(IEntity entity, int index)
		{

		}

		public IEntity RemoveManager(IEntity entity)
		{
			if(entity == null)
				return null;
			return RemoveManager(entity, entity.GetComponentType(this));
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
				if(managers.IsEmpty && isAutoDisposed)
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
			if(!isShareable) //One manager.
			{
				Engine = entity.Engine; //One engine.
			}
			else //Many managers.
			{
				if(engine != null)
				{
					if(engine != entity.Engine)
						Engine = null; //Different engines.
				}
				else
				{
					IEngine engine = managers.First.Value.Engine;
					if(engine == null)
						return;
					for(ILinkListNode<IEntity> current = managers.First.Next; current != null;)
					{
						if(current.Value.Engine != engine)
							return;
						current = current.Next;
					}
					Engine = engine; //Same engines.
				}
			}
		}

		public IEngine Engine
		{
			get
			{
				return engine;
			}
			private set
			{
				if(engine == value)
					return;
				IEngine previous = engine;
				engine = value;
				ChangingEngine(value, previous);
				engineChanged.Dispatch(this, value, previous);
			}
		}

		public ISignal<IComponent, IEngine, IEngine> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		protected virtual void ChangingEngine(IEngine current, IEngine previous)
		{

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
		public string ToString(bool addManagers = true, int index = 0, string indent = "")
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
			text.AppendLine(indent + "  Auto Dispose = " + isAutoDisposed);
			text.AppendLine(indent + "  Managers (" + managers.Count + ")");
			if(addManagers)
			{
				index = 0;
				ILinkListNode<IEntity> current = managers.First;
				while(current != null)
				{
					IEntity manager = current.Value;
					text.AppendLine(indent + "    Manager " + (++index));
					text.AppendLine(indent + "      Abstraction = " + manager.GetComponentType(this).FullName);
					text.AppendLine(indent + "      GlobalName  = " + manager.GlobalName);
					text.AppendLine(indent + "      Hierarchy   = " + manager.HierarchyToString());
					current = current.Next;
				}
			}
			return text.ToString();
		}
	}

	abstract class AtlasComponent<TBaseAbstraction>:AtlasComponent, IComponent<TBaseAbstraction> where TBaseAbstraction : IComponent
	{
		public AtlasComponent()
		{

		}

		public AtlasComponent(bool isShareable = false) : base(isShareable)
		{

		}

		public IEntity AddManager<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction
		{
			return AddManager(entity, typeof(TAbstraction));
		}

		public IEntity AddManager<TAbstraction>(IEntity entity, int index) where TAbstraction : TBaseAbstraction
		{
			return AddManager(entity, typeof(TAbstraction), index);
		}

		public IEntity RemoveManager<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction
		{
			return RemoveManager(entity, typeof(TAbstraction));
		}
	}
}