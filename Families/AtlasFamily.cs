using Atlas.Components;
using Atlas.Engine;
using Atlas.Entities;
using Atlas.LinkList;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Families
{
	sealed class AtlasFamily:IFamily
	{
		private IEngine engine;
		private Signal<IFamily, IEngine, IEngine> engineChanged = new Signal<IFamily, IEngine, IEngine>();

		private IFamilyType familyType;
		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private HashSet<Type> componentSet = new HashSet<Type>();
		private HashSet<IEntity> entitySet = new HashSet<IEntity>();

		private Signal<IFamily, IEntity> entityAdded = new Signal<IFamily, IEntity>();
		private Signal<IFamily, IEntity> entityRemoved = new Signal<IFamily, IEntity>();

		private bool isDisposed = false;

		public static implicit operator bool(AtlasFamily nodelist)
		{
			return nodelist != null;
		}

		public AtlasFamily()
		{

		}

		public void Dispose()
		{
			if(engine != null)
				return;
			FamilyType = null;
		}

		public IEngine Engine
		{
			get
			{
				return engine;
			}
			set
			{
				if(value != null)
				{
					if(engine == null && value.HasFamily(this))
					{
						IEngine previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(engine != null && !engine.HasFamily(this))
					{
						IEngine previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

		public Signal<IFamily, IEngine, IEngine> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		public IFamilyType FamilyType
		{
			get
			{
				return familyType;
			}
			set
			{
				if(familyType != null)
					return;
				if(familyType != value)
				{
					familyType = value;
					foreach(Type type in familyType.Components)
					{
						componentSet.Add(type);
					}
				}
			}
		}

		public IReadOnlyLinkList<IEntity> Entities
		{
			get
			{
				return entities;
			}
		}

		public Signal<IFamily, IEntity> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public Signal<IFamily, IEntity> EntityRemoved
		{
			get
			{
				return entityRemoved;
			}
		}

		public void AddEntity(IEntity entity)
		{
			Add(entity);
		}

		public void RemoveEntity(IEntity entity)
		{
			Remove(entity);
		}

		public void AddEntity(IEntity entity, IComponent component, Type componentType)
		{
			if(componentSet.Contains(componentType))
			{
				Add(entity);
			}
		}

		public void RemoveEntity(IEntity entity, IComponent component, Type componentType)
		{
			if(componentSet.Contains(componentType))
			{
				Remove(entity);
			}
		}

		private void Add(IEntity entity)
		{
			if(!entitySet.Contains(entity))
			{
				if(familyType.Components.Count > 0)
				{
					foreach(Type componentType in familyType.Components)
					{
						if(!entity.HasComponent(componentType))
						{
							return;
						}
					}
				}

				entities.Add(entity);
				entitySet.Add(entity);

				entityAdded.Dispatch(this, entity);
			}
		}

		private void Remove(IEntity entity)
		{
			if(entitySet.Contains(entity))
			{
				entityRemoved.Dispatch(this, entity);

				entitySet.Remove(entity);
				entities.Remove(entity);

				//nodesRemoved.Add(node);
			}
		}
		/*
		public void DisposeNodes()
		{
			while(nodesRemoved.Count > 0)
			{

				Node node = nodesRemoved[nodesRemoved.Count - 1];
				nodesRemoved.RemoveAt(nodesRemoved.Count - 1);
				DisposeNode(node);
			}
		}

		private void DisposeNode(Node node)
		{
			foreach(Type componentType in components.Keys)
			{
				FieldInfo field = componentType.GetField(components[componentType]);
				field.SetValue(node, null);
			}

			//node.NodeList = null;
			node.Entity = null;
			node.Previous = null;
			node.Next = null;

			nodesPooled.Add(node);
		}*/

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
				}
			}
		}

		public bool IsDisposedWhenUnmanaged
		{
			get
			{
				return true;
			}
			set
			{

			}
		}
	}
}
