using Atlas.Entities;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Components
{
	abstract class Component:IComponent
	{
		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private Signal<IComponent, IEntity, int> entityAdded = new Signal<IComponent, IEntity, int>();
		private Signal<IComponent, IEntity, int> entityRemoved = new Signal<IComponent, IEntity, int>();

		private bool isShareable = false;

		private bool isDisposedWhenUnmanaged = true;
		private bool isDisposed = false;
		private Signal<Component, bool> isDisposedChanged = new Signal<Component, bool>();

		public static implicit operator bool(Component component)
		{
			return component != null;
		}

		public Component() : this(false)
		{

		}

		public Component(bool isShareable = false)
		{
			IsShareable = isShareable;
		}

		public void Dispose()
		{
			if(entities.Count > 0)
			{
				IsDisposedWhenUnmanaged = true;
				RemoveComponentManagers();
			}
			else
			{
				Disposing();
				IsDisposed = true;
				IsDisposedWhenUnmanaged = true;
				isDisposedChanged.Dispose();
				entityAdded.Dispose();
				entityRemoved.Dispose();
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
					isDisposedChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<Component, bool> IsDisposedChanged
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
				isDisposedWhenUnmanaged = value;
			}
		}

		public bool IsShareable
		{
			get
			{
				return isShareable;
			}
			private set
			{
				isShareable = value;
			}
		}

		public Signal<IComponent, IEntity, int> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public Signal<IComponent, IEntity, int> EntityRemoved
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
				return entities.Count == 1 ? entities.First.Value : null;
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

		public bool SwapEntities(Entity entity1, Entity entity2)
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
				{
					type = GetType();
				}
				else if(!type.IsInstanceOfType(this))
				{
					return null;
				}
				if(entity.GetComponent(type) == this)
				{
					index = Math.Max(0, Math.Min(index, entities.Count));
					entities.Add(entity, index);
					IsDisposed = false;
					AddingComponentManager(entity);
					entityAdded.Dispatch(this, entity, index);
				}
				else
				{
					entity.AddComponent(this, type, index);
				}
			}
			else
			{
				SetEntityIndex(entity, index);
			}
			return entity;
		}

		protected virtual void AddingComponentManager(IEntity entity)
		{

		}

		public IEntity RemoveEntity(IEntity entity)
		{
			if(entity == null)
				return null;
			if(!entities.Contains(entity))
				return null;
			Type type = entity.GetComponentType(this);
			if(type == null)
			{
				int index = entities.GetIndex(entity);
				RemovingComponentManager(entity);
				entities.Remove(index);
				entityRemoved.Dispatch(this, entity, index);
				if(entities.Count <= 0 && isDisposedWhenUnmanaged)
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

		public IEntity RemoveEntity(int index)
		{
			if(index < 0)
				return null;
			if(index > entities.Count - 1)
				return null;
			return RemoveEntity(entities[index]);
		}

		protected virtual void RemovingComponentManager(IEntity entity)
		{

		}

		public void RemoveComponentManagers()
		{
			while(entities.Count > 0)
			{
				RemoveEntity(entities[entities.Count - 1]);
			}
		}

		public string Dump(string indent = "")
		{
			return "";
		}
	}
}