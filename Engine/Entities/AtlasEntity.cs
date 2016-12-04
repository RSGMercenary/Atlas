using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Engine.Entities
{
	class AtlasEntity:IEntity
	{
		private IEngine engine;
		private string globalName = Guid.NewGuid().ToString("N");
		private string localName = Guid.NewGuid().ToString("N");
		private int sleeping = 0;
		private int freeSleeping = 0;
		private IEntity root;
		private IEntity parent;
		private int parentIndex = -1;
		private LinkList<IEntity> children = new LinkList<IEntity>();
		private Dictionary<string, IEntity> childrenLocalName = new Dictionary<string, IEntity>();
		private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		private HashSet<Type> systems = new HashSet<Type>();
		private bool isAutoDisposed = true;
		private bool isDisposing = false;

		private Signal<IEntity, IEngine, IEngine> engineChanged = new Signal<IEntity, IEngine, IEngine>();
		private Signal<IEntity, string, string> globalNameChanged = new Signal<IEntity, string, string>();
		private Signal<IEntity, string, string> localNameChanged = new Signal<IEntity, string, string>();
		private Signal<IEntity, IEntity, IEntity, IEntity> rootChanged = new Signal<IEntity, IEntity, IEntity, IEntity>();
		private Signal<IEntity, IEntity, IEntity> parentChanged = new Signal<IEntity, IEntity, IEntity>();
		private Signal<IEntity, int, int> parentIndexChanged = new Signal<IEntity, int, int>(); //Index within parent
		private Signal<IEntity, IEntity, int> childAdded = new Signal<IEntity, IEntity, int>();
		private Signal<IEntity, IEntity, int> childRemoved = new Signal<IEntity, IEntity, int>();
		private Signal<IEntity, int, int, bool> childIndicesChanged = new Signal<IEntity, int, int, bool>();
		private Signal<IEntity, IComponent, Type> componentAdded = new Signal<IEntity, IComponent, Type>();
		private Signal<IEntity, IComponent, Type> componentRemoved = new Signal<IEntity, IComponent, Type>();
		private Signal<IEntity, Type> systemAdded = new Signal<IEntity, Type>();
		private Signal<IEntity, Type> systemRemoved = new Signal<IEntity, Type>();
		private Signal<IEntity, int, int, IEntity> sleepingChanged = new Signal<IEntity, int, int, IEntity>();
		private Signal<IEntity, int, int> freeSleepingChanged = new Signal<IEntity, int, int>();
		private Signal<IEntity> disposed = new Signal<IEntity>();

		public static implicit operator bool(AtlasEntity entity)
		{
			return entity != null;
		}

		public AtlasEntity()
		{

		}

		public AtlasEntity(string globalName = "", string localName = "")
		{
			GlobalName = globalName;
			LocalName = localName;
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
			Parent = null;
			Sleeping = 0;
			FreeSleeping = 0;
			//Doing this for the Engine as it's self-referencing.
			//If it's not nulled, GC might not pick it up.(?)
			root = null;
			engineChanged.Dispose();
			globalNameChanged.Dispose();
			localNameChanged.Dispose();
			parentChanged.Dispose();
			parentIndexChanged.Dispose();
			childAdded.Dispose();
			childRemoved.Dispose();
			childIndicesChanged.Dispose();
			componentAdded.Dispose();
			componentRemoved.Dispose();
			systemAdded.Dispose();
			systemRemoved.Dispose();
			sleepingChanged.Dispose();
			freeSleepingChanged.Dispose();
			disposed.Dispatch(this);
			disposed.Dispose();
		}

		public void Reset()
		{
			Resetting();
		}

		protected virtual void Resetting()
		{
			RemoveChildren();
			RemoveComponents();
			RemoveSystemTypes();
			GlobalName = Guid.NewGuid().ToString("N");
			LocalName = Guid.NewGuid().ToString("N");
			IsAutoDisposed = true;
		}

		public ISignal<IEntity, IEngine, IEngine> EngineChanged { get { return engineChanged; } }
		public ISignal<IEntity, string, string> GlobalNameChanged { get { return globalNameChanged; } }
		public ISignal<IEntity, string, string> LocalNameChanged { get { return localNameChanged; } }
		public ISignal<IEntity, IEntity, IEntity, IEntity> RootChanged { get { return rootChanged; } }
		public ISignal<IEntity, IEntity, IEntity> ParentChanged { get { return parentChanged; } }
		public ISignal<IEntity, int, int> ParentIndexChanged { get { return parentIndexChanged; } }
		public ISignal<IEntity, IEntity, int> ChildAdded { get { return childAdded; } }
		public ISignal<IEntity, IEntity, int> ChildRemoved { get { return childRemoved; } }
		public ISignal<IEntity, int, int, bool> ChildIndicesChanged { get { return childIndicesChanged; } }
		public ISignal<IEntity, IComponent, Type> ComponentAdded { get { return componentAdded; } }
		public ISignal<IEntity, IComponent, Type> ComponentRemoved { get { return componentRemoved; } }
		public ISignal<IEntity, Type> SystemTypeAdded { get { return systemAdded; } }
		public ISignal<IEntity, Type> SystemTypeRemoved { get { return systemRemoved; } }
		public ISignal<IEntity, int, int, IEntity> SleepingChanged { get { return sleepingChanged; } }
		public ISignal<IEntity, int, int> FreeSleepingChanged { get { return freeSleepingChanged; } }
		public ISignal<IEntity> Disposed { get { return disposed; } }

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
				if(parent == null && isAutoDisposed)
					Dispose();
			}
		}

		#region Entity

		public string GlobalName
		{
			get
			{
				return globalName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					return;
				if(globalName == value)
					return;
				if(engine != null && engine.HasEntity(value))
					return;
				string previous = globalName;
				globalName = value;
				globalNameChanged.Dispatch(this, value, previous);
			}
		}

		public string LocalName
		{
			get
			{
				return localName;
			}
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					return;
				if(localName == value)
					return;
				if(parent != null && parent.HasChild(value))
					return;
				string previous = localName;
				localName = value;
				localNameChanged.Dispatch(this, value, previous);
			}
		}

		public string HierarchyToString()
		{
			if(parent != null)
			{
				return parent.HierarchyToString() + "/" + localName;
			}
			else
			{
				return localName;
			}
		}

		public IEntity GetHierarchy(string hierarchy)
		{
			if(string.IsNullOrWhiteSpace(hierarchy))
				return null;
			string[] localNames = hierarchy.Split('/');
			IEntity entity = this;
			foreach(string localName in localNames)
			{
				if(string.IsNullOrWhiteSpace(localName))
					continue;
				if(localName == "..")
				{
					entity = entity.Parent;
				}
				else
				{
					entity = entity.GetChild(localName);
				}
				if(entity == null)
				{
					break;
				}
			}
			return entity;
		}

		public bool SetHierarchy(string hierarchy, int index)
		{
			return SetParent(GetHierarchy(hierarchy), index);
		}

		public IEntity GetChild(string localName)
		{
			return childrenLocalName.ContainsKey(localName) ? childrenLocalName[localName] : null;
		}

		public IReadOnlyDictionary<string, IEntity> ChildLocalNames
		{
			get { return childrenLocalName; }
		}

		#endregion

		#region Engine

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
					if(engine == null && value.HasEntity(this))
					{
						IEngine previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(engine != null && !engine.HasEntity(this))
					{
						IEngine previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

		#endregion

		#region Components

		public bool HasComponent<TAbstraction>() where TAbstraction : IComponent
		{
			return HasComponent(typeof(TAbstraction));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		public TComponent GetComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction
		{
			return (TComponent)GetComponent(typeof(TAbstraction));
		}

		public TAbstraction GetComponent<TAbstraction>() where TAbstraction : IComponent
		{
			return (TAbstraction)GetComponent(typeof(TAbstraction));
		}

		public IComponent GetComponent(Type type)
		{
			return components.ContainsKey(type) ? components[type] : null;
		}

		public Type GetComponentType(IComponent component)
		{
			if(component == null)
				return null;
			foreach(Type type in components.Keys)
			{
				if(components[type] == component)
				{
					return type;
				}
			}
			return null;
		}

		public IReadOnlyDictionary<Type, IComponent> Components
		{
			get
			{
				return components;
			}
		}

		//New component with Type
		public TComponent AddComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction, new()
		{
			return (TComponent)AddComponent(new TComponent(), typeof(TAbstraction), 0);
		}

		//Component with Type
		public TComponent AddComponent<TComponent, TAbstraction>(TComponent component) where TComponent : IComponent, TAbstraction
		{
			return (TComponent)AddComponent(component, typeof(TAbstraction), int.MaxValue);
		}

		//Component with Type, index
		public TComponent AddComponent<TComponent, TAbstraction>(TComponent component, int index)
			where TComponent : IComponent, TAbstraction
		{
			return (TComponent)AddComponent(component, typeof(TAbstraction), index);
		}

		//New Component
		public TComponent AddComponent<TComponent>() where TComponent : IComponent, new()
		{
			return (TComponent)AddComponent(new TComponent(), null, 0);
		}

		//Component
		public TComponent AddComponent<TComponent>(TComponent component) where TComponent : IComponent
		{
			return (TComponent)AddComponent(component, null, int.MaxValue);
		}

		//Component, index
		public TComponent AddComponent<TComponent>(TComponent component, int index) where TComponent : IComponent
		{
			return (TComponent)AddComponent(component, null, index);
		}

		public IComponent AddComponent(IComponent component, Type type)
		{
			return AddComponent(component, type, int.MaxValue);
		}

		public IComponent AddComponent(IComponent component, int index)
		{
			return AddComponent(component, null, index);
		}

		public IComponent AddComponent(IComponent component, Type type = null, int index = int.MaxValue)
		{
			if(component == null)
				return null;
			if(component.Entity != null)
				return null;
			if(type == null)
				type = component.GetType();
			else if(type == typeof(IComponent))
				return null;
			else if(!type.IsInstanceOfType(component))
				return null;
			if(!components.ContainsKey(type) || components[type] != component)
			{
				RemoveComponent(type);
				components.Add(type, component);
				component.AddEntity(this, type, index);
				componentAdded.Dispatch(this, component, type);
			}
			return component;
		}

		public TComponent RemoveComponent<TComponent, TAbstraction>() where TComponent : IComponent, TAbstraction
		{
			return (TComponent)RemoveComponent(typeof(TAbstraction));
		}

		public TAbstraction RemoveComponent<TAbstraction>() where TAbstraction : IComponent
		{
			return (TAbstraction)RemoveComponent(typeof(TAbstraction));
		}

		public IComponent RemoveComponent(Type type)
		{
			if(type == null)
				return null;
			if(!components.ContainsKey(type))
				return null;
			IComponent component = components[type];
			components.Remove(type);
			component.RemoveEntity(this, type);
			componentRemoved.Dispatch(this, component, type);
			return component;
		}

		public IComponent RemoveComponent(IComponent component)
		{
			return RemoveComponent(GetComponentType(component));
		}

		public bool RemoveComponents()
		{
			if(components.Count <= 0)
				return false;
			//Can't Remove() from a Dictionary while iterating its Keys.
			Type[] types = new Type[components.Count];
			components.Keys.CopyTo(types, 0);
			foreach(Type type in types)
			{
				RemoveComponent(type);
			}
			return true;
		}

		#endregion

		#region Hierarchy

		public IEntity Root
		{
			get
			{
				return root;
			}
		}

		private void SetRoot(IEntity value, IEntity source)
		{
			if(root == value)
				return;
			IEntity previous = root;
			root = value;
			rootChanged.Dispatch(this, value, previous, source);
		}

		private void ParentRootChanged(IEntity parent, IEntity next, IEntity previous, IEntity source)
		{
			SetRoot(next, source);
		}

		public bool HasChild(string localName)
		{
			return GetChild(localName) != null;
		}

		private IEntity GetEntity(string globalName, string localName)
		{
			return engine != null ? engine.GetEntity(true, globalName, localName) : new AtlasEntity();
		}

		public IEntity AddChild(string globalName = "", string localName = "")
		{
			return AddChild(GetEntity(globalName, localName), children.Count);
		}

		public IEntity AddChild(int index, string globalName = "", string localName = "")
		{
			return AddChild(GetEntity(globalName, localName), index);
		}

		public IEntity Parent
		{
			get { return parent; }
			set { SetParent(value); }
		}

		public bool HasChild(IEntity child)
		{
			return children.Contains(child);
		}

		public IEntity AddChild(IEntity child)
		{
			return AddChild(child, children.Count);
		}

		public IEntity AddChild(IEntity child, int index)
		{
			if(child == null)
				return null;
			if(child.Parent == this)
			{
				if(childrenLocalName.ContainsKey(child.LocalName) && childrenLocalName[child.LocalName] != child)
				{
					child.LocalName = Guid.NewGuid().ToString("N");
				}
				if(!childrenLocalName.ContainsKey(child.LocalName))
				{
					childrenLocalName.Add(child.LocalName, child);
					children.Add(child, index);
					child.LocalNameChanged.Add(ChildLocalNameChanged, int.MinValue);
					childAdded.Dispatch(this, child, index);
					childIndicesChanged.Dispatch(this, index, children.Count - 1, true);
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				if(!child.SetParent(this, index))
					return null;
			}
			return child;
		}

		private void ChildLocalNameChanged(IEntity child, string next, string previous)
		{
			childrenLocalName.Remove(previous);
			childrenLocalName.Add(next, child);
		}

		public IEntity RemoveChild(IEntity child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!childrenLocalName.ContainsKey(child.LocalName))
					return null;
				int index = children.GetIndex(child);
				children.Remove(index);
				childrenLocalName.Remove(child.LocalName);
				child.LocalNameChanged.Remove(ChildLocalNameChanged);
				childRemoved.Dispatch(this, child, index);
				childIndicesChanged.Dispatch(this, index, children.Count, true);
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public IEntity RemoveChild(int index)
		{
			if(index < 0)
				return null;
			if(index > children.Count - 1)
				return null;
			return RemoveChild(children.Get(index));
		}

		public IEntity RemoveChild(string localName)
		{
			if(!childrenLocalName.ContainsKey(localName))
				return null;
			return RemoveChild(childrenLocalName[localName]);
		}

		public bool RemoveChildren()
		{
			if(children.IsEmpty)
				return false;
			while(!children.IsEmpty)
			{
				children.Last.Value.Dispose();
			}
			return true;
		}

		public bool SetParent(IEntity nextParent = null, int index = int.MaxValue)
		{
			if(this == Root)
				return false;
			if(parent == nextParent)
				return false;
			//Can't set a descendant of this as a parent.
			if(HasDescendant(nextParent))
				return false;
			IEntity previousParent = parent;
			int sleeping = 0;
			IEntity source = null;
			IEntity root = this.root;
			if(previousParent != null)
			{
				parent = null;
				previousParent.RootChanged.Remove(ParentRootChanged);
				previousParent.ChildIndicesChanged.Remove(ParentChildIndicesChanged);
				previousParent.SleepingChanged.Remove(ParentSleepingChanged);
				previousParent.RemoveChild(parentIndex);
				if(!IsFreeSleeping && previousParent.IsSleeping)
					--sleeping;
				root = null;
				source = previousParent;
			}
			if(nextParent != null)
			{
				parent = nextParent;
				index = Math.Max(0, Math.Min(index, nextParent.Children.Count));
				nextParent.AddChild(this, index);
				nextParent.RootChanged.Add(ParentRootChanged, int.MinValue);
				nextParent.ChildIndicesChanged.Add(ParentChildIndicesChanged, int.MinValue + index);
				nextParent.SleepingChanged.Add(ParentSleepingChanged, int.MinValue + index);
				if(!IsFreeSleeping && nextParent.IsSleeping)
					++sleeping;
				root = nextParent.Root;
				source = nextParent;
			}
			else
			{
				index = -1;
			}
			parentChanged.Dispatch(this, nextParent, previousParent);
			SetParentIndex(index);
			SetSleeping(this.sleeping + sleeping, source);
			SetRoot(root, source);
			if(parent == null && isAutoDisposed)
			{
				Dispose();
			}
			return true;
		}

		public int ParentIndex
		{
			get
			{
				return parentIndex;
			}
			set
			{
				if(parent != null)
					parent.SetChildIndex(this, value);
			}
		}

		public bool HasDescendant(IEntity descendant)
		{
			while(descendant != null && descendant != this)
			{
				descendant = descendant.Parent;
			}
			return descendant == this;
		}

		public bool HasAncestor(IEntity ancestor)
		{
			return ancestor != null ? ancestor.HasDescendant(this) : false;
		}

		public IEntity GetChild(int index)
		{
			if(index < 0)
				return null;
			if(index > children.Count - 1)
				return null;
			return children.Get(index);
		}

		public int GetChildIndex(IEntity child)
		{
			return children.GetIndex(child);
		}

		public bool SetChildIndex(IEntity child, int index)
		{
			int previous = children.GetIndex(child);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			int next = index;

			children.Remove(previous);
			children.Add(child, next);

			if(next > previous)
			{
				childIndicesChanged.Dispatch(this, previous, next, true);
			}
			else
			{
				childIndicesChanged.Dispatch(this, next, previous, true);
			}
			return true;
		}

		public bool SwapChildren(AtlasEntity child1, AtlasEntity child2)
		{
			if(child1 == null)
				return false;
			if(child2 == null)
				return false;
			int index1 = children.GetIndex(child1);
			int index2 = children.GetIndex(child2);
			return SwapChildren(index1, index2);
		}

		public bool SwapChildren(int index1, int index2)
		{
			if(!children.Swap(index1, index2))
				return false;
			childIndicesChanged.Dispatch(this, Math.Min(index1, index2), Math.Max(index1, index2), false);
			return true;
		}

		public IReadOnlyLinkList<IEntity> Children
		{
			get { return children; }
		}

		private void ParentChildIndicesChanged(IEntity parent, int min, int max, bool inclusive)
		{
			if(inclusive)
			{
				if(parentIndex >= min && parentIndex <= max)
				{
					SetParentIndex(parent.GetChildIndex(this));
				}
			}
			else
			{
				if(parentIndex == min || parentIndex == max)
				{
					SetParentIndex(parent.GetChildIndex(this));
				}
			}
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			if(parent != null)
			{
				parent.ChildIndicesChanged.Get(ParentChildIndicesChanged).Priority = int.MinValue + parentIndex;
				parent.SleepingChanged.Get(ParentSleepingChanged).Priority = int.MinValue + parentIndex;
			}
			parentIndexChanged.Dispatch(this, value, previous);
		}

		#endregion

		#region Sleep

		public int Sleeping
		{
			get
			{
				return sleeping;
			}
			set
			{
				SetSleeping(value, this);
			}
		}

		private void SetSleeping(int value, IEntity source)
		{
			if(sleeping == value)
				return;
			int previous = sleeping;
			sleeping = value;
			sleepingChanged.Dispatch(this, value, previous, source);
		}

		private void ParentSleepingChanged(IEntity parent, int next, int previous, IEntity source)
		{
			if(next > 0 && previous <= 0)
			{
				SetSleeping(sleeping + 1, source);
			}
			else if(next <= 0 && previous > 0)
			{
				SetSleeping(sleeping - 1, source);
			}
		}

		public bool IsSleeping
		{
			get
			{
				return sleeping > 0;
			}
		}

		public int FreeSleeping
		{
			get
			{
				return freeSleeping;
			}
			set
			{
				if(freeSleeping != value)
				{
					int previous = freeSleeping;
					freeSleeping = value;
					freeSleepingChanged.Dispatch(this, value, previous);
					if(parent == null)
						return;
					if(value > 0 && previous <= 0)
					{
						parent.SleepingChanged.Remove(ParentSleepingChanged);
						if(parent.IsSleeping)
							SetSleeping(sleeping - 1, parent);
					}
					else if(value <= 0 && previous > 0)
					{
						parent.SleepingChanged.Add(ParentSleepingChanged, int.MinValue + parentIndex);
						if(parent.IsSleeping)
							SetSleeping(sleeping + 1, parent);
					}
				}
			}
		}

		public bool IsFreeSleeping
		{
			get
			{
				return freeSleeping > 0;
			}
		}

		#endregion

		#region Systems

		public IReadOnlyCollection<Type> SystemTypes
		{
			get
			{
				return (IReadOnlyCollection<Type>)systems;
			}
		}

		public bool HasSystemType<TSystem>() where TSystem : ISystem
		{
			return HasSystemType(typeof(TSystem));
		}

		public bool HasSystemType(Type type)
		{
			return systems.Contains(type);
		}

		public bool AddSystemType<TSystem>() where TSystem : ISystem
		{
			return AddSystemType(typeof(TSystem));
		}

		public bool AddSystemType(Type type)
		{
			if(!systems.Contains(type))
			{
				systems.Add(type);
				systemAdded.Dispatch(this, type);
				return true;
			}
			return false;
		}

		public bool RemoveSystemType<TSystem>() where TSystem : ISystem
		{
			return RemoveSystemType(typeof(TSystem));
		}

		public bool RemoveSystemType(Type type)
		{
			if(systems.Contains(type))
			{
				systems.Remove(type);
				systemRemoved.Dispatch(this, type);
				return true;
			}
			return false;
		}

		public bool RemoveSystemTypes()
		{
			if(systems.Count <= 0)
				return false;
			Type[] types = new Type[systems.Count];
			systems.CopyTo(types, 0);
			foreach(Type system in types)
			{
				RemoveSystemType(system);
			}
			return true;
		}

		#endregion

		public override string ToString()
		{
			return ToString(-1, true, true, false);
		}

		public string ToString(int depth, bool addComponents, bool addEntities, bool addSystems, string indent = "")
		{
			StringBuilder text = new StringBuilder();

			text.AppendLine(indent + "Child " + (parentIndex + 1));
			text.AppendLine(indent + "  Instance      = " + GetType().FullName);
			text.AppendLine(indent + "  Global Name   = " + globalName);
			text.AppendLine(indent + "  Local Name    = " + localName);
			text.AppendLine(indent + "  Sleeping      = " + sleeping);
			text.AppendLine(indent + "  Free Sleeping = " + freeSleeping);
			text.AppendLine(indent + "  Auto Dispose  = " + IsAutoDisposed);

			text.AppendLine(indent + "  Components (" + components.Count + ")");
			if(addComponents)
			{
				int index = 0;
				foreach(Type type in components.Keys)
				{
					IComponent component = components[type];
					text.Append(component.ToString(addEntities, ++index, indent + "    "));
				}
			}

			text.AppendLine(indent + "  Systems    (" + systems.Count + ")");
			if(addSystems)
			{
				int index = 0;
				foreach(Type type in systems)
				{
					text.AppendLine(indent + "    System " + (++index) + " " + type.FullName);
				}
			}

			text.AppendLine(indent + "  Children   (" + children.Count + ")");
			if(depth != 0)
			{
				for(var current = children.First; current != null; current = current.Next)
				{
					text.Append(current.Value.ToString(depth - 1, addComponents, addSystems, addEntities, indent + "    "));
				}
			}
			return text.ToString();
		}
	}
}