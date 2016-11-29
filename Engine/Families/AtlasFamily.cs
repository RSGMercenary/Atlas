using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atlas.Engine.Families
{
	sealed class AtlasFamily:IFamily
	{
		private IEngine engine;
		private Signal<IFamily, IEngine, IEngine> engineChanged = new Signal<IFamily, IEngine, IEngine>();

		private Type familyType;
		private LinkList<IEntity> entities = new LinkList<IEntity>();
		private HashSet<IEntity> entitySet = new HashSet<IEntity>();
		private List<Type> components = new List<Type>();
		private HashSet<Type> componentsSet = new HashSet<Type>();

		private Signal<IFamily, IEntity> entityAdded = new Signal<IFamily, IEntity>();
		private Signal<IFamily, IEntity> entityRemoved = new Signal<IFamily, IEntity>();

		private bool isDisposed = false;
		private Signal<IFamily, bool> isDisposedChanged = new Signal<IFamily, bool>();

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
			engineChanged.Dispose();
			entityAdded.Dispose();
			entityRemoved.Dispose();
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

		public ISignal<IFamily, IEngine, IEngine> EngineChanged
		{
			get
			{
				return engineChanged;
			}
		}

		public Type FamilyType
		{
			get
			{
				return familyType;
			}
			set
			{
				if(familyType != null)
					return;
				if(familyType == value)
					return;

				familyType = value;

				foreach(FieldInfo info in familyType.GetFields())
				{
					Type component = info.FieldType;
					components.Add(component);
					componentsSet.Add(component);
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

		public ISignal<IFamily, IEntity> EntityAdded
		{
			get
			{
				return entityAdded;
			}
		}

		public ISignal<IFamily, IEntity> EntityRemoved
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
			if(componentsSet.Contains(componentType))
			{
				Add(entity);
			}
		}

		public void RemoveEntity(IEntity entity, IComponent component, Type componentType)
		{
			if(componentsSet.Contains(componentType))
			{
				Remove(entity);
			}
		}

		private void Add(IEntity entity)
		{
			if(entitySet.Contains(entity))
				return;
			foreach(Type type in components)
			{
				if(!entity.HasComponent(type))
					return;
			}
			entities.Add(entity);
			entitySet.Add(entity);
			entityAdded.Dispatch(this, entity);
		}

		private void Remove(IEntity entity)
		{
			if(!entitySet.Contains(entity))
				return;
			entities.Remove(entity);
			entitySet.Remove(entity);
			entityRemoved.Dispatch(this, entity);
			//nodesRemoved.Add(node);
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
					isDisposedChanged.Dispatch(this, value);
				}
			}
		}

		public ISignal<IFamily, bool> IsDisposedChanged
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
				return true;
			}
			set
			{

			}
		}
	}
}
