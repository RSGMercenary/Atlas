using Atlas.Entities;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Components
{
	abstract class Component
	{
		private List<Entity> componentManagers = new List<Entity>();
		private Signal<Component, Entity> componentManagerAdded = new Signal<Component, Entity>();
		private Signal<Component, Entity> componentManagerRemoved = new Signal<Component, Entity>();

		private bool isShareable = false;

		private bool isDisposedWhenUnmanaged = true;
		private bool isDisposed = false;
		private Signal<Component, bool> isDisposedChanged = new Signal<Component, bool>();

		public Component() : this(false)
		{

		}

		public Component(bool isShareable = false)
		{
			IsShareable = isShareable;
		}

		public void Dispose()
		{
			if(componentManagers.Count > 0)
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
				componentManagerAdded.Dispose();
				componentManagerRemoved.Dispose();
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

		public Signal<Component, Entity> ComponentManagerAdded
		{
			get
			{
				return componentManagerAdded;
			}
		}

		public Signal<Component, Entity> ComponentManagerRemoved
		{
			get
			{
				return componentManagerRemoved;
			}
		}

		public Entity ComponentManager
		{
			get
			{
				if(componentManagers.Count == 1)
				{
					return componentManagers[0];
				}
				return null;
			}
		}

		public List<Entity> ComponentManagers
		{
			get
			{
				return new List<Entity>(componentManagers);
			}
		}

		public int NumComponentManagers
		{
			get
			{
				return componentManagers.Count;
			}
		}

		public int GetComponentManagerIndex(Entity entity)
		{
			return componentManagers.IndexOf(entity);
		}

		public bool SetComponentManagerIndex(Entity entity, int index)
		{
			if(index < 0)
				return false;
			if(index > componentManagers.Count - 1)
				return false;

			int previous = componentManagers.IndexOf(entity);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			//-1 is needed since technically you lost a child on the previous splice.
			if(index > previous)
			{
				--index;
			}

			componentManagers.RemoveAt(previous);
			componentManagers.Insert(index, entity);
			return true;
		}

		public bool SwapComponentManagers(Entity entity1, Entity entity2)
		{
			if(entity1 == null)
				return false;
			if(entity2 == null)
				return false;
			int index1 = componentManagers.IndexOf(entity1);
			int index2 = componentManagers.IndexOf(entity2);
			return SwapComponentManagers(index1, index2);
		}

		public bool SwapComponentManagers(int index1, int index2)
		{
			if(index1 < 0 || index1 > componentManagers.Count - 1)
				return false;
			if(index2 < 0 || index2 > componentManagers.Count - 1)
				return false;
			Entity entity = componentManagers[index1];
			componentManagers[index1] = componentManagers[index2];
			componentManagers[index2] = entity;
			return true;
		}

		public Entity AddComponentManager(Entity entity, Type type)
		{
			return AddComponentManager(entity, type, componentManagers.Count);
		}

		public Entity AddComponentManager(Entity entity, int index)
		{
			return AddComponentManager(entity, null, index);
		}

		public Entity AddComponentManager(Entity entity, Type type = null, int index = int.MaxValue)
		{
			if(entity != null)
			{
				if(!componentManagers.Contains(entity))
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
						index = Math.Max(0, Math.Min(index, componentManagers.Count));
						componentManagers.Insert(index, entity);
						IsDisposed = false;
						AddingComponentManager(entity);
						componentManagerAdded.Dispatch(this, entity);
					}
					else
					{
						entity.AddComponent(this, type, index);
					}
					return entity;
				}
			}
			return null;
		}

		protected virtual void AddingComponentManager(Entity entity)
		{

		}

		public Entity RemoveComponentManager(Entity entity)
		{
			if(entity != null)
			{
				if(componentManagers.Contains(entity))
				{
					Type componentType = entity.GetComponentType(this);
					if(componentType == null)
					{
						componentManagerRemoved.Dispatch(this, entity);
						//It's possible that the Entity could've been removed during the Dispatch().
						int index = componentManagers.IndexOf(entity);
						if(index > -1)
						{
							RemovingComponentManager(entity);
							componentManagers.RemoveAt(index);
							if(componentManagers.Count == 0 && isDisposedWhenUnmanaged)
							{
								Dispose();
							}
						}
					}
					else
					{
						entity.RemoveComponent(componentType);
					}
					return entity;
				}
			}
			return null;
		}

		public Entity RemoveComponentManager(int index)
		{
			if(index < 0)
				return null;
			if(index > componentManagers.Count - 1)
				return null;
			return RemoveComponentManager(componentManagers[index]);
		}

		protected virtual void RemovingComponentManager(Entity entity)
		{

		}

		public void RemoveComponentManagers()
		{
			while(componentManagers.Count > 0)
			{
				RemoveComponentManager(componentManagers[componentManagers.Count - 1]);
			}
		}

		public string Dump(string indent = "")
		{
			return "";
		}
	}
}