using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent:IComponent
	{
		private readonly bool isShareable = false;
		private LinkList<IEntity> managers = new LinkList<IEntity>();
		private bool isDisposed = false;
		private bool isAutoDisposed = true;
		IEngineManager engine;

		private Signal<IComponent, IEngineManager, IEngineManager> engineChanged = new Signal<IComponent, IEngineManager, IEngineManager>();
		private Signal<IComponent, IEntity, int> managerAdded = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, IEntity, int> managerRemoved = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, bool, bool> isDisposedChanged = new Signal<IComponent, bool, bool>();

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
			if(!managers.IsEmpty)
			{
				IsAutoDisposed = true;
				RemoveManagers();
			}
			else
			{
				Disposing();
				IsDisposed = true;
				IsAutoDisposed = true;
				managerAdded.Dispose();
				managerRemoved.Dispose();
				isDisposedChanged.Dispose();
			}
		}

		protected virtual void Disposing()
		{

		}

		public bool IsDisposed
		{
			get
			{
				return isDisposed;
			}
			private set
			{
				if(isDisposed != value)
				{
					bool previous = isDisposed;
					isDisposed = value;
					isDisposedChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<IComponent, bool, bool> IsDisposedChanged
		{
			get
			{
				return isDisposedChanged;
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
				if(isAutoDisposed != value)
				{
					isAutoDisposed = value;

					if(!isDisposed && managers.IsEmpty && isAutoDisposed)
					{
						Dispose();
					}
				}
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
				return (isShareable && !managers.IsEmpty) ? managers.First.Value : null;
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
					IsDisposed = false;
					index = Math.Max(0, Math.Min(index, managers.Count));
					managers.Add(entity, index);
					entity.EngineChanged.Add(EntityEngineChanged, int.MinValue);
					EntityEngineChanged(entity);
					AddingManager(entity, index);
					managerAdded.Dispatch(this, entity, index);
				}
				else
				{
					//TO-DO Pass back null if the AddComponent() isn't successful.
					entity.AddComponent(this, type, index);
				}
			}
			else
			{
				SetManagerIndex(entity, index);
			}
			return entity;
		}

		private void EntityEngineChanged(IEntity entity, IEngineManager next = null, IEngineManager previous = null)
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
					IEngineManager engine = managers.First.Value.Engine;
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

		public IEngineManager Engine
		{
			get
			{
				return engine;
			}
			private set
			{
				if(engine != value)
				{
					IEngineManager previous = engine;
					engine = value;
					ChangingEngine(value, previous);
				}
			}
		}

		public ISignal<IComponent, IEngineManager, IEngineManager> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		protected virtual void ChangingEngine(IEngineManager current, IEngineManager previous)
		{

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
				{
					Dispose();
				}
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

		public override string ToString()
		{
			string text = "";

			text += "Component";
			text += "\n  ";
			text += "Instance     = " + GetType().FullName;
			text += "\n  ";
			text += "Shareable    = " + isShareable;
			text += "\n  ";
			text += "Disposed     = " + isDisposed;
			text += "\n  ";
			text += "Auto-Dispose = " + isAutoDisposed;

			if(!managers.IsEmpty)
			{
				text += "\n  ";
				text += "Managers";
				int index = 0;
				for(ILinkListNode<IEntity> current = managers.First; current != null;)
				{
					IEntity entity = current.Value;
					text += "\n    ";
					text += "Manager " + (++index);
					text += "\n      ";
					text += "Abstraction = " + entity.GetComponentType(this).FullName;
					text += "\n      ";
					text += "GlobalName  = " + entity.GlobalName;
					text += "\n      ";
					text += "LocalName   = " + entity.LocalName;
					current = current.Next;
				}
			}
			return text;
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