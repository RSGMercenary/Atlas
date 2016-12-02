using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Text;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent:IComponent
	{
		private IEngine engine;
		private readonly bool isShareable = false;
		private bool isAutoDisposed = true;
		private bool isDisposing = false;
		private LinkList<IEntity> entities = new LinkList<IEntity>();

		private Signal<IComponent, IEngine, IEngine> engineChanged = new Signal<IComponent, IEngine, IEngine>();
		private Signal<IComponent, IEntity, int> entityAdded = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, IEntity, int> entityRemoved = new Signal<IComponent, IEntity, int>();
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
			Reset();
			engineChanged.Dispose();
			entityAdded.Dispose();
			entityRemoved.Dispose();
			disposed.Dispatch(this);
			disposed.Dispose();
		}

		public void Reset()
		{
			Resetting();
		}

		protected virtual void Resetting()
		{
			RemoveEntities();
			IsAutoDisposed = true;
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
				if(entities.IsEmpty && isAutoDisposed)
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

		public ISignal<IComponent, IEntity, int> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public ISignal<IComponent, IEntity, int> EntityRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public IEntity Entity
		{
			get
			{
				return (!isShareable && !entities.IsEmpty) ? entities.First.Value : null;
			}
		}

		public IReadOnlyLinkList<IEntity> Entities
		{
			get
			{
				return entities;
			}
		}

		public int GetEntityIndex(IEntity entity)
		{
			return entities.GetIndex(entity);
		}

		public bool SetEntityIndex(IEntity entity, int index)
		{
			return entities.SetIndex(entity, index);
		}

		public bool SwapEntities(AtlasEntity entity1, AtlasEntity entity2)
		{
			return entities.Swap(entity1, entity2);
		}

		public bool SwapComponentManagers(int index1, int index2)
		{
			return entities.Swap(index1, index2);
		}

		public IEntity AddEntity(IEntity entity)
		{
			return AddEntity(entity, null);
		}

		public IEntity AddEntity(IEntity entity, Type type)
		{
			return AddEntity(entity, type, entities.Count);
		}

		public IEntity AddEntity(IEntity entity, int index)
		{
			return AddEntity(entity, null, index);
		}

		public IEntity AddEntity(IEntity entity, Type type = null, int index = int.MaxValue)
		{
			if(entity == null)
				return null;
			if(!entities.Contains(entity))
			{
				if(type == null)
					type = GetType();
				else if(type == typeof(IComponent))
					return null;
				else if(!type.IsInstanceOfType(this))
					return null;
				if(entity.GetComponent(type) == this)
				{
					index = Math.Max(0, Math.Min(index, entities.Count));
					entities.Add(entity, index);
					entity.EngineChanged.Add(EntityEngineChanged, int.MinValue);
					Engine = entity.Engine;
					AddingManager(entity, index);
					entityAdded.Dispatch(this, entity, index);
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
				SetEntityIndex(entity, index);
			}
			return entity;
		}

		protected virtual void AddingManager(IEntity entity, int index)
		{

		}

		public IEntity RemoveEntity(IEntity entity)
		{
			if(entity == null)
				return null;
			return RemoveEntity(entity, entity.GetComponentType(this));
		}

		public IEntity RemoveEntity(IEntity entity, Type type)
		{
			if(entity == null)
				return null;
			if(!entities.Contains(entity))
				return null;
			if(type == null)
				type = GetType();
			else if(type == typeof(IComponent))
				return null;
			else if(!type.IsInstanceOfType(this))
				return null;
			if(entity.GetComponent(type) == null)
			{
				int index = entities.GetIndex(entity);
				entities.Remove(index);
				entity.EngineChanged.Remove(EntityEngineChanged);
				if(entities.IsEmpty)
					Engine = null;
				RemovingManager(entity, index);
				entityRemoved.Dispatch(this, entity, index);
				if(entities.IsEmpty && isAutoDisposed)
					Dispose();
			}
			else
			{
				entity.RemoveComponent(type);
			}
			return entity;
		}

		public IEntity RemoveEntity(int index)
		{
			if(index < 0)
				return null;
			if(index > entities.Count - 1)
				return null;
			return RemoveEntity(entities[index]);
		}

		protected virtual void RemovingManager(IEntity entity, int index)
		{

		}

		public bool RemoveEntities()
		{
			if(entities.IsEmpty)
				return false;
			while(!entities.IsEmpty)
				RemoveEntity(entities.Last.Value);
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
					if(engine != entity.Root)
						Engine = null; //Different engines.
				}
				else
				{
					IEngine engine = entities.First.Value.Engine;
					if(engine == null)
						return;
					for(var current = entities.First.Next; current != null; current = current.Next)
					{
						if(current.Value.Root != engine)
							return;
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
			text.AppendLine(indent + "  Managers (" + entities.Count + ")");
			if(addManagers)
			{
				index = 0;
				for(var current = entities.First; current != null; current = current.Next)
				{
					IEntity manager = current.Value;
					text.AppendLine(indent + "    Manager " + (++index));
					text.AppendLine(indent + "      Abstraction = " + manager.GetComponentType(this).FullName);
					text.AppendLine(indent + "      GlobalName  = " + manager.GlobalName);
					text.AppendLine(indent + "      Hierarchy   = " + manager.HierarchyToString());
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

		public IEntity AddEntity<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction
		{
			return AddEntity(entity, typeof(TAbstraction));
		}

		public IEntity AddEntity<TAbstraction>(IEntity entity, int index) where TAbstraction : TBaseAbstraction
		{
			return AddEntity(entity, typeof(TAbstraction), index);
		}

		public IEntity RemoveEntity<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction
		{
			return RemoveEntity(entity, typeof(TAbstraction));
		}
	}
}