using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Systems;
using Atlas.Engine.LinkList;
using Atlas.Engine.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Engine.Entities
{
	sealed class AtlasEntity:IEntity
	{
		private IEngineManager engine;
		private string globalName = new Guid().ToString("N");
		private string localName = new Guid().ToString("N");
		private IEntity parent;
		private LinkList<IEntity> children = new LinkList<IEntity>();
		private Dictionary<string, IEntity> childLocalNames = new Dictionary<string, IEntity>();
		private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		private HashSet<Type> systems = new HashSet<Type>();
		private int sleeping = 0;
		private int freeSleeping = 0;
		private bool isDisposed = false;
		private bool isAutoDisposed = true;

		private Signal<IEntity, IEngineManager, IEngineManager> engineChanged = new Signal<IEntity, IEngineManager, IEngineManager>();
		private Signal<IEntity, string, string> globalNameChanged = new Signal<IEntity, string, string>();
		private Signal<IEntity, string, string> localNameChanged = new Signal<IEntity, string, string>();
		private Signal<IEntity, IEntity, IEntity> parentChanged = new Signal<IEntity, IEntity, IEntity>();
		private Signal<IEntity, int, int> parentIndexChanged = new Signal<IEntity, int, int>(); //Index within parent
		private Signal<IEntity, IEntity, int> childAdded = new Signal<IEntity, IEntity, int>();
		private Signal<IEntity, IEntity, int> childRemoved = new Signal<IEntity, IEntity, int>();
		private Signal<IEntity, int, int, bool> childIndicesChanged = new Signal<IEntity, int, int, bool>(); //Indices of children
		private Signal<IEntity, IComponent, Type> componentAdded = new Signal<IEntity, IComponent, Type>();
		private Signal<IEntity, IComponent, Type> componentRemoved = new Signal<IEntity, IComponent, Type>();
		private Signal<IEntity, Type> systemAdded = new Signal<IEntity, Type>();
		private Signal<IEntity, Type> systemRemoved = new Signal<IEntity, Type>();
		private Signal<IEntity, int, int> sleepingChanged = new Signal<IEntity, int, int>();
		private Signal<IEntity, int, int> freeSleepingChanged = new Signal<IEntity, int, int>();
		private Signal<IEntity, bool, bool> isDisposedChanged = new Signal<IEntity, bool, bool>();

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
			if(engine != null)
			{
				IsAutoDisposed = true;
				Parent = null;
			}
			else
			{
				IsDisposed = true;
				RemoveChildren();
				RemoveComponents();
				Parent = null;
				Sleeping = 0;
				FreeSleeping = 0;
				IsAutoDisposed = true;
				GlobalName = "";
				LocalName = "";

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
			}
		}

		public ISignal<IEntity, IEngineManager, IEngineManager> EngineChanged { get { return engineChanged; } }
		public ISignal<IEntity, string, string> GlobalNameChanged { get { return globalNameChanged; } }
		public ISignal<IEntity, string, string> LocalNameChanged { get { return localNameChanged; } }
		public ISignal<IEntity, IEntity, IEntity> ParentChanged { get { return parentChanged; } }
		public ISignal<IEntity, int, int> ParentIndexChanged { get { return parentIndexChanged; } }
		public ISignal<IEntity, IEntity, int> ChildAdded { get { return childAdded; } }
		public ISignal<IEntity, IEntity, int> ChildRemoved { get { return childRemoved; } }
		public ISignal<IEntity, int, int, bool> ChildIndicesChanged { get { return childIndicesChanged; } }
		public ISignal<IEntity, IComponent, Type> ComponentAdded { get { return componentAdded; } }
		public ISignal<IEntity, IComponent, Type> ComponentRemoved { get { return componentRemoved; } }
		public ISignal<IEntity, Type> SystemAdded { get { return systemAdded; } }
		public ISignal<IEntity, Type> SystemRemoved { get { return systemRemoved; } }
		public ISignal<IEntity, int, int> SleepingChanged { get { return sleepingChanged; } }
		public ISignal<IEntity, int, int> FreeSleepingChanged { get { return freeSleepingChanged; } }
		public ISignal<IEntity, bool, bool> IsDisposedChanged { get { return isDisposedChanged; } }

		public IEngineManager Engine
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
						IEngineManager previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(engine != null && !engine.HasEntity(this))
					{
						IEngineManager previous = engine;
						engine = value;
						engineChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

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

		public IEntity Root
		{
			get
			{
				return engine != null ? engine.Manager : null;
			}
		}

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
		public TComponent AddComponent<TComponent, TAbstraction>(TComponent component, int index) where TComponent : IComponent, TAbstraction
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
			if(component.Manager != null)
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
				component.AddManager(this, type, index);
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
			component.RemoveManager(this, type);
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
			foreach(Type type in components.Keys)
			{
				RemoveComponent(type);
			}
			return true;
		}

		public IEntity Parent
		{
			get
			{
				return parent;
			}
			set
			{
				SetParent(value);
			}
		}

		public bool HasChild(IEntity child)
		{
			return children.Contains(child);
		}

		public bool HasChild(string localName)
		{
			return GetChild(localName) != null;
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
				if(childLocalNames.ContainsKey(child.LocalName) && childLocalNames[child.LocalName] != child)
				{
					child.LocalName = new Guid().ToString("N");
				}
				if(!childLocalNames.ContainsKey(child.LocalName))
				{
					childLocalNames.Add(child.LocalName, child);
					children.Add(child, index);
					child.LocalNameChanged.Add(ChildLocalNameChanged);
					if(IsSleeping && !child.IsFreeSleeping)
					{
						++child.Sleeping;
					}
					childAdded.Dispatch(this, child, index);
					childIndicesChanged.Dispatch(this, index, children.Count, true);
					for(int i = index + 1; i < children.Count; ++i)
					{
						IEntity sibling = children[i];
						sibling.ParentIndexChanged.Dispatch(sibling, i, i - 1);
					}
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				child.SetParent(this, index);
			}
			return child;
		}

		private void ChildLocalNameChanged(IEntity child, string next, string previous)
		{
			childLocalNames.Remove(previous);
			childLocalNames.Add(next, child);
		}

		public IEntity RemoveChild(IEntity child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!childLocalNames.ContainsKey(child.LocalName))
					return null;
				int index = children.GetIndex(child);
				children.Remove(index);
				childLocalNames.Remove(child.LocalName);
				child.LocalNameChanged.Remove(ChildLocalNameChanged);
				childRemoved.Dispatch(this, child, index);
				childIndicesChanged.Dispatch(this, index, children.Count, true);

				for(int i = index; i < children.Count; ++i)
				{
					IEntity sibling = children[i];
					sibling.ParentIndexChanged.Dispatch(sibling, i, i + 1);
				}
				if(IsSleeping && !child.IsFreeSleeping)
				{
					--child.Sleeping;
				}
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

		public bool SetParent(IEntity parent = null, int index = int.MaxValue)
		{
			//Can't set the parent of the root.
			if(parent != null && Root == this)
				return false;
			//Parents are the same.
			if(this.parent == parent)
				return false;
			//Can't set a parent's ancestor (this) as a child.
			if(HasHierarchy(parent))
				return false;
			IEntity previousParent = this.parent;
			int previousIndex = -1;
			this.parent = parent;
			if(previousParent != null)
			{
				previousIndex = previousParent.GetChildIndex(this);
				previousParent.RemoveChild(previousIndex);
			}
			if(parent != null)
			{
				IsDisposed = false;
				index = Math.Max(0, Math.Min(index, parent.Children.Count));
				parent.AddChild(this, index);
			}
			else
			{
				index = -1;
			}
			parentChanged.Dispatch(this, parent, previousParent);
			if(index != previousIndex)
			{
				parentIndexChanged.Dispatch(this, index, previousIndex);
			}
			if(this.parent == null && isAutoDisposed)
			{
				/*
				 * If an Entity is set as the Root, but it has a parent,
				 * then we need to remove its parent without disposing.
				 */
				if(Root != this)
					Dispose();
			}
			return true;
		}

		public bool HasHierarchy(IEntity relative)
		{
			while(relative != null && relative != this)
			{
				relative = relative.Parent;
			}
			return relative == this;
		}

		public IEntity GetHierarchy(string hierarchy)
		{
			if(string.IsNullOrWhiteSpace(hierarchy))
				return null;
			string[] localNames = hierarchy.Split('/');
			IEntity entity = this;
			foreach(string localName in localNames)
			{
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
			return childLocalNames.ContainsKey(localName) ? childLocalNames[localName] : null;
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

				//Children shift down 0<-[1]
				for(index = previous; index < next; ++index)
				{
					child = children[index];
					child.ParentIndexChanged.Dispatch(child, index, index + 1);
				}
			}
			else
			{
				childIndicesChanged.Dispatch(this, next, previous, true);

				//Children shift up [0]->1
				for(index = previous; index > next; --index)
				{
					child = children[index];
					child.ParentIndexChanged.Dispatch(child, index, index - 1);
				}
			}
			child.ParentIndexChanged.Dispatch(child, next, previous);
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
			if(index1 < 0)
				return false;
			if(index2 < 0)
				return false;
			if(index1 > children.Count - 1)
				return false;
			if(index2 > children.Count - 1)
				return false;
			IEntity child1 = children[index1];
			IEntity child2 = children[index2];
			children.Swap(child1, child2);
			childIndicesChanged.Dispatch(this, Math.Min(index1, index2), Math.Max(index1, index2), false);
			child1.ParentIndexChanged.Dispatch(child1, index2, index1);
			child2.ParentIndexChanged.Dispatch(child2, index1, index2);
			return true;
		}

		public IReadOnlyLinkList<IEntity> Children
		{
			get
			{
				return children;
			}
		}

		public IReadOnlyDictionary<string, IEntity> ChildLocalNames
		{
			get
			{
				return childLocalNames;
			}
		}

		public int Sleeping
		{
			get
			{
				return sleeping;
			}
			set
			{
				if(sleeping != value)
				{
					int previous = sleeping;
					sleeping = value;
					sleepingChanged.Dispatch(this, value, previous);

					if(value > 0 && previous <= 0)
					{
						ILinkListNode<IEntity> current = children.First;
						while(current != null)
						{
							if(!current.Value.IsFreeSleeping)
							{
								++current.Value.Sleeping;
							}
							current = current.Next;
						}
					}
					else if(value <= 0 && previous > 0)
					{
						ILinkListNode<IEntity> current = children.First;
						while(current != null)
						{
							if(!current.Value.IsFreeSleeping)
							{
								--current.Value.Sleeping;
							}
							current = current.Next;
						}
					}
				}
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

					if(parent != null && parent.IsSleeping)
					{
						if(value <= 0)
						{
							++Sleeping;
						}
						else
						{
							--Sleeping;
						}
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

		public IReadOnlyCollection<Type> Systems
		{
			get
			{
				return (IReadOnlyCollection<Type>)systems;
			}
		}

		public bool HasSystem<TSystem>() where TSystem : ISystem
		{
			return HasSystem(typeof(TSystem));
		}

		public bool HasSystem(Type type)
		{
			return systems != null && systems.Contains(type);
		}

		public bool AddSystem<TSystem>() where TSystem : ISystem
		{
			return AddSystem(typeof(TSystem));
		}

		public bool AddSystem(Type type)
		{
			if(!systems.Contains(type))
			{
				systems.Add(type);
				systemAdded.Dispatch(this, type);
				return true;
			}
			return false;
		}

		public bool RemoveSystem<TSystem>() where TSystem : ISystem
		{
			return RemoveSystem(typeof(TSystem));
		}

		public bool RemoveSystem(Type type)
		{
			if(systems.Contains(type))
			{
				systems.Remove(type);
				systemRemoved.Dispatch(this, type);
				return true;
			}
			return false;
		}

		public bool RemoveSystems()
		{
			if(systems.Count <= 0)
				return false;
			foreach(Type system in systems)
			{
				RemoveSystem(system);
			}
			return true;
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

					if(!isDisposed && parent == null && isAutoDisposed)
					{
						Dispose();
					}
				}
			}
		}

		public override string ToString()
		{
			return ToString(true, true, true);
		}

		public string ToString(bool includeChildren, bool includeComponents, bool includeSystems, string indent = "")
		{
			string text = indent;

			text += "Child " + (parent != null ? parent.GetChildIndex(this) + 1 : 1);
			text += "\n  " + indent;
			text += "Global Name   = " + globalName;
			text += "\n  " + indent;
			text += "Local Name    = " + localName;
			text += "\n  " + indent;
			text += "Sleeping      = " + sleeping;
			text += "\n  " + indent;
			text += "Free-Sleeping = " + freeSleeping;
			text += "\n  " + indent;
			text += "Auto-Dispose  = " + isAutoDisposed;

			if(includeComponents && components.Count > 0)
			{
				text += "\n  " + indent;
				text += "Components";
				int index = 0;
				foreach(Type type in components.Keys)
				{
					IComponent component = components[type];
					text += "\n    " + indent;
					text += "Component " + (++index);
					text += "\n      " + indent;
					text += "Abstraction  = " + type.FullName;
					text += "\n      " + indent;
					text += "Instance     = " + component.GetType().FullName;
					text += "\n      " + indent;
					text += "Managers     = " + component.Managers.Count;
					text += "\n      " + indent;
					text += "Shareable    = " + component.IsShareable;
					text += "\n      " + indent;
					text += "Auto-Dispose = " + component.IsAutoDisposed;
				}
			}

			if(includeSystems && systems.Count > 0)
			{
				text += "\n  " + indent;
				text += "Systems";
				int index = 0;
				foreach(Type type in systems)
				{
					text += "\n    " + indent;
					text += "System " + (++index);
					text += "\n      " + indent;
					text += "Type = " + type.FullName;
				}
			}

			if(includeChildren)
			{
				if(!children.IsEmpty)
				{
					text += "\n  " + indent;
					text += "Children";
					text += "\n";
					for(int index = 0; index < children.Count; ++index)
					{
						text += children[index].ToString(includeChildren, includeComponents, includeSystems, indent + "    ");
					}
				}
				else
				{
					text += "\n";
				}
			}
			return text;
		}
	}
}
