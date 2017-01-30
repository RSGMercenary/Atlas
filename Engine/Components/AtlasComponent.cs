using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Text;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent : BaseObject<IComponent>, IComponent
	{
		private IEngine engine;
		private readonly bool isShareable = false;
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

		protected override void Disposing()
		{
			Reset();
			engineChanged.Dispose();
			entityAdded.Dispose();
			entityRemoved.Dispose();
			base.Disposing();
		}

		public void Reset()
		{
			Resetting();
		}

		protected virtual void Resetting()
		{
			RemoveManagers();
			AutoDispose = true;
		}

		protected override void ChangingAutoDispose()
		{
			base.ChangingAutoDispose();
			if (AutoDispose && entities.IsEmpty)
				Dispose();
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
				return entityAdded;
			}
		}

		public ISignal<IComponent, IEntity, int> ManagerRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public IEntity Manager
		{
			get
			{
				return !isShareable ? entities.First?.Value : null;
			}
		}

		public IReadOnlyLinkList<IEntity> Managers
		{
			get
			{
				return entities;
			}
		}

		public int GetManagerIndex(IEntity entity)
		{
			return entities.GetIndex(entity);
		}

		public bool SetManagerIndex(IEntity entity, int index)
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

		public IEntity AddManager(IEntity entity)
		{
			return AddManager(entity, null);
		}

		public IEntity AddManager(IEntity entity, Type type)
		{
			return AddManager(entity, type, entities.Count);
		}

		public IEntity AddManager(IEntity entity, int index)
		{
			return AddManager(entity, null, index);
		}

		public IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue)
		{
			if (entity == null)
				return null;
			if (!entities.Contains(entity))
			{
				if (type == null)
					type = GetType();
				else if (type == typeof(IComponent))
					return null;
				else if (!type.IsInstanceOfType(this))
					return null;
				if (entity.GetComponent(type) == this)
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
					if (entity.AddComponent(this, type, index) == null)
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
			if (entity == null)
				return null;
			return RemoveManager(entity, entity.GetComponentType(this));
		}

		public IEntity RemoveManager(IEntity entity, Type type)
		{
			if (entity == null)
				return null;
			if (!entities.Contains(entity))
				return null;
			if (type == null)
				type = GetType();
			else if (type == typeof(IComponent))
				return null;
			else if (!type.IsInstanceOfType(this))
				return null;
			if (entity.GetComponent(type) == null)
			{
				int index = entities.GetIndex(entity);
				entities.Remove(index);
				entity.EngineChanged.Remove(EntityEngineChanged);
				if (entities.IsEmpty)
					Engine = null;
				RemovingManager(entity, index);
				entityRemoved.Dispatch(this, entity, index);
				if (AutoDispose && entities.IsEmpty)
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
			if (index < 0)
				return null;
			if (index > entities.Count - 1)
				return null;
			return RemoveManager(entities[index]);
		}

		protected virtual void RemovingManager(IEntity entity, int index)
		{

		}

		public bool RemoveManagers()
		{
			if (entities.IsEmpty)
				return false;
			while (!entities.IsEmpty)
				RemoveManager(entities.Last.Value);
			return true;
		}

		private void EntityEngineChanged(IEntity entity, IEngine next = null, IEngine previous = null)
		{
			if (!isShareable) //One manager.
			{
				Engine = entity.Engine; //One engine.
			}
			else //Many managers.
			{
				if (engine != null)
				{
					if (engine != entity.Root)
						Engine = null; //Different engines.
				}
				else
				{
					IEngine engine = entities.First.Value.Engine;
					if (engine == null)
						return;
					foreach (IEntity other in entities)
					{
						if (other.Root != engine)
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
				if (engine == value)
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
		public string ToString(bool addEntities = true, int index = 0, string indent = "")
		{
			StringBuilder text = new StringBuilder();

			text.Append(indent + "Component");
			if (index > 0)
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
			text.AppendLine(indent + "  Auto Dispose = " + AutoDispose);
			text.AppendLine(indent + "  Entities (" + entities.Count + ")");
			if (addEntities)
			{
				index = 0;
				foreach (IEntity entity in entities)
				{
					text.AppendLine(indent + "    Entity " + (++index));
					text.AppendLine(indent + "      Abstraction = " + entity.GetComponentType(this).FullName);
					text.AppendLine(indent + "      GlobalName  = " + entity.GlobalName);
					text.AppendLine(indent + "      Hierarchy   = " + entity.HierarchyToString());
				}
			}
			return text.ToString();
		}
	}
}