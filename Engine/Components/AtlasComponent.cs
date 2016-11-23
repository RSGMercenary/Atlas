﻿using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Engine.Components
{
	abstract class AtlasComponent<TBaseInterface>:AtlasComponent, IComponent<TBaseInterface> where TBaseInterface : IComponent
	{
		public AtlasComponent() : base(false)
		{

		}

		public AtlasComponent(bool isShareable = false) : base(isShareable)
		{

		}

		public IEntity AddManager<TInterface>(IEntity entity) where TInterface : TBaseInterface
		{
			return AddManager(entity, typeof(TInterface));
		}

		public IEntity AddManager<TInterface>(IEntity entity, int index) where TInterface : TBaseInterface
		{
			return AddManager(entity, typeof(TInterface), index);
		}
	}

	abstract class AtlasComponent:IComponent
	{
		private readonly bool isShareable = false;
		private LinkList<IEntity> managers = new LinkList<IEntity>();
		private bool isDisposed = false;
		private bool isDisposedWhenUnmanaged = true;
		IEngineManager engine;

		private ISignal<IComponent, IEntity, int> managerAdded = new Signal<IComponent, IEntity, int>();
		private ISignal<IComponent, IEntity, int> managerRemoved = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, bool, bool> isDisposedChanged = new Signal<IComponent, bool, bool>();

		public static implicit operator bool(AtlasComponent component)
		{
			return component != null;
		}

		public AtlasComponent() : this(false)
		{

		}

		public AtlasComponent(bool isShareable = false)
		{
			this.isShareable = isShareable;
		}

		public void Dispose()
		{
			if(managers.Count > 0)
			{
				IsDisposedWhenUnmanaged = true;
				RemoveManagers();
			}
			else
			{
				Disposing();
				IsDisposed = true;
				IsDisposedWhenUnmanaged = true;
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

		public bool IsDisposedWhenUnmanaged
		{
			get
			{
				return isDisposedWhenUnmanaged;
			}
			set
			{
				if(isDisposedWhenUnmanaged != value)
				{
					isDisposedWhenUnmanaged = value;

					if(!isDisposed && managers.Count <= 0 && value)
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
				return managers.Count == 1 ? managers.First.Value : null;
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
			if(managers.Count == 1) //One manager.
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
					if(managers.First == null)
						return;
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

		protected void ChangingEngine(IEngineManager current, IEngineManager previous)
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
				if(managers.Count <= 0 && isDisposedWhenUnmanaged)
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
			if(managers.Count <= 0)
				return false;
			while(managers.Count > 0)
				RemoveManager(managers.Last.Value);
			return true;
		}

		public string Dump(string indent = "")
		{
			return "";
		}
	}
}