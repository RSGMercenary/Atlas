using Atlas.Components;
using Atlas.LinkList;
using Atlas.Signals;
using Atlas.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Entities
{
	sealed class Entity:IEntity
	{
		private IEntityManager entityManager;
		private Signal<IEntity, IEntityManager, IEntityManager> entityManagerChanged = new Signal<IEntity, IEntityManager, IEntityManager>();

		private string globalName = Guid.NewGuid().ToString("N");
		private Signal<IEntity, string, string> globalNameChanged = new Signal<IEntity, string, string>();

		private string localName = Guid.NewGuid().ToString("N");
		private Signal<IEntity, string, string> localNameChanged = new Signal<IEntity, string, string>(false);

		private LinkList<IEntity> children = new LinkList<IEntity>();
		private Dictionary<string, IEntity> childLocalNames = new Dictionary<string, IEntity>();
		private Signal<IEntity, IEntity, int> childAdded = new Signal<IEntity, IEntity, int>();
		private Signal<IEntity, IEntity, int> childRemoved = new Signal<IEntity, IEntity, int>();
		//Bool is true for inclusive (1 through 4) and false for exclusive (1 and 4)
		private Signal<IEntity, int, int, bool> childIndicesChanged = new Signal<IEntity, int, int, bool>(); //Indices of children


		private IEntity parent;
		private Signal<IEntity, IEntity, IEntity> parentChanged = new Signal<IEntity, IEntity, IEntity>();
		private Signal<IEntity, int, int> parentIndexChanged = new Signal<IEntity, int, int>(); //Index within parent

		private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		private Signal<IEntity, IComponent, Type> componentAdded = new Signal<IEntity, IComponent, Type>();
		private Signal<IEntity, IComponent, Type> componentRemoved = new Signal<IEntity, IComponent, Type>();

		private HashSet<Type> systemTypes = new HashSet<Type>();
		private Signal<IEntity, Type> systemTypeAdded = new Signal<IEntity, Type>();
		private Signal<IEntity, Type> systemTypeRemoved = new Signal<IEntity, Type>();

		private int sleeping = 0;
		private Signal<Entity, int, int> sleepingChanged = new Signal<Entity, int, int>();

		private int sleepingParentIgnored = 0;
		private Signal<Entity, int, int> sleepingParentIgnoredChanged = new Signal<Entity, int, int>();

		private bool isDisposed = false;
		private bool isDisposedWhenUnmanaged = true;

		public static implicit operator bool(Entity entity)
		{
			return entity != null;
		}

		public Entity(string uniqueName = "", string name = "")
		{
			GlobalName = uniqueName;
			LocalName = name;
		}

		public void Dispose()
		{
			if(entityManager != null)
			{
				IsDisposedWhenUnmanaged = true;
				Parent = null;
			}
			else
			{
				IsDisposed = true;
				RemoveChildren();
				RemoveComponents();
				Parent = null;
				Sleeping = 0;
				SleepingParentIgnored = 0;
				IsDisposedWhenUnmanaged = true;
				GlobalName = "";
				LocalName = "";

				entityManagerChanged.Dispose();
				globalNameChanged.Dispose();
				localNameChanged.Dispose();
				childAdded.Dispose();
				childRemoved.Dispose();
				parentChanged.Dispose();
				componentAdded.Dispose();
				componentRemoved.Dispose();
				sleepingChanged.Dispose();
				sleepingParentIgnoredChanged.Dispose();
			}
		}

		public IEntityManager EntityManager
		{
			get
			{
				return entityManager;
			}
			set
			{
				if(value != null)
				{
					if(entityManager == null && value.HasEntity(this))
					{
						IEntityManager previous = entityManager;
						entityManager = value;
						entityManagerChanged.Dispatch(this, value, previous);
					}
				}
				else
				{
					if(entityManager != null && !entityManager.HasEntity(this))
					{
						IEntityManager previous = entityManager;
						entityManager = value;
						entityManagerChanged.Dispatch(this, value, previous);
					}
				}
			}
		}

		public Signal<IEntity, IEntityManager, IEntityManager> EntityManagerChanged
		{
			get
			{
				return entityManagerChanged;
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
				if(entityManager != null && entityManager.HasEntity(value))
					return;
				string previous = globalName;
				globalName = value;
				globalNameChanged.Dispatch(this, value, previous);
			}
		}

		public Signal<IEntity, string, string> GlobalNameChanged
		{
			get
			{
				return globalNameChanged;
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

		public Signal<IEntity, string, string> LocalNameChanged
		{
			get
			{
				return localNameChanged;
			}
		}

		public IEntity Root
		{
			get
			{
				return entityManager != null ? entityManager.Entity : null;
			}
		}

		public bool HasComponent<T>() where T : IComponent
		{
			return HasComponent(typeof(T));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		public T GetComponent<T>() where T : IComponent
		{
			return (T)GetComponent(typeof(T));
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

		public T AddComponent<T>() where T : IComponent, new()
		{
			return (T)AddComponent(new T());
		}

		public T AddComponent<T>(T component) where T : IComponent
		{
			return (T)AddComponent(component, typeof(T));
		}

		public T AddComponent<T>(T component, int index) where T : IComponent
		{
			return (T)AddComponent(component, typeof(T), index);
		}

		public IComponent AddComponent(IComponent component)
		{
			return AddComponent(component, null);
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
			if(!component.IsShareable && component.Entities.Count > 0)
				return null;
			if(type == null)
			{
				type = component.GetType();
			}
			else if(!type.IsInstanceOfType(component))
			{
				return null;
			}
			if(!components.ContainsKey(type) || components[type] != component)
			{
				RemoveComponent(type);
				components.Add(type, component);
				component.AddEntity(this, type, index);
				componentAdded.Dispatch(this, component, type);
			}
			return component;
		}

		public Signal<IEntity, IComponent, Type> ComponentAdded
		{
			get
			{
				return componentAdded;
			}
		}

		public T RemoveComponent<T>() where T : IComponent
		{
			return (T)RemoveComponent(typeof(T));
		}

		public IComponent RemoveComponent(Type type)
		{
			if(type == null)
				return null;
			if(!components.ContainsKey(type))
				return null;
			IComponent component = components[type];
			components.Remove(type);
			component.RemoveEntity(this);
			componentRemoved.Dispatch(this, component, type);
			return component;
		}

		public Signal<IEntity, IComponent, Type> ComponentRemoved
		{
			get
			{
				return componentRemoved;
			}
		}

		public IComponent RemoveComponent(IComponent component)
		{
			return RemoveComponent(GetComponentType(component));
		}

		public void RemoveComponents()
		{
			foreach(Type type in components.Keys)
			{
				RemoveComponent(type);
			}
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
					child.LocalName = Guid.NewGuid().ToString("N");
				}
				if(!childLocalNames.ContainsKey(child.LocalName))
				{
					childLocalNames.Add(child.LocalName, child);
					children.Add(child, index);
					child.LocalNameChanged.Add(ChildLocalNameChanged);
					if(IsSleeping && !child.IsSleepingParentIgnored)
					{
						++child.Sleeping;
					}
					childAdded.Dispatch(this, child, index);
					childIndicesChanged.Dispatch(this, index, children.Count, true);
					for(int i = index; i < children.Count; ++i)
					{
						IEntity sibling = children[i];
						sibling.ParentIndexChanged.Dispatch(sibling, i + 1, i);
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

		public Signal<IEntity, IEntity, int> ChildAdded
		{
			get
			{
				return childAdded;
			}
		}

		public IEntity RemoveChild(IEntity child)
		{
			if(child != null)
			{
				if(child.Parent != this)
				{
					if(childLocalNames.ContainsKey(child.LocalName))
					{
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
						if(IsSleeping && !child.IsSleepingParentIgnored)
						{
							--child.Sleeping;
						}
						return child;
					}
				}
				else
				{
					child.SetParent(null, -1);
					return child;
				}
			}
			return null;
		}

		public Signal<IEntity, IEntity, int> ChildRemoved
		{
			get
			{
				return childRemoved;
			}
		}

		public IEntity RemoveChild(int index)
		{
			if(index < 0)
				return null;
			if(index > children.Count - 1)
				return null;
			return RemoveChild(children.Get(index));
		}

		public void RemoveChildren()
		{
			while(children.First != null)
			{
				children.Last.Value.Dispose();
			}
		}

		public bool SetParent(IEntity parent = null, int index = int.MaxValue)
		{
			if(this.parent == parent)
				return false;
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

			if(parent == null && IsDisposedWhenUnmanaged)
			{
				Dispose();
			}
			return true;
		}

		public Signal<IEntity, IEntity, IEntity> ParentChanged
		{
			get
			{
				return parentChanged;
			}
		}

		public bool HasHierarchy(IEntity entity)
		{
			while(entity != this && entity != null)
			{
				entity = entity.Parent;
			}
			return entity == this;
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

		public Signal<IEntity, int, int> ParentIndexChanged
		{
			get
			{
				return parentIndexChanged;
			}
		}

		public Signal<IEntity, int, int, bool> ChildIndicesChanged
		{
			get
			{
				return childIndicesChanged;
			}
		}

		public bool SwapChildren(Entity child1, Entity child2)
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

		public Signal<Entity, int, int> SleepingChanged
		{
			get
			{
				return sleepingChanged;
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
							if(!current.Value.IsSleepingParentIgnored)
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
							if(!current.Value.IsSleepingParentIgnored)
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

		public Signal<Entity, int, int> SleepingParentIgnoredChanged
		{
			get
			{
				return sleepingParentIgnoredChanged;
			}
		}

		public int SleepingParentIgnored
		{
			get
			{
				return sleepingParentIgnored;
			}
			set
			{
				if(sleepingParentIgnored != value)
				{
					int previous = sleepingParentIgnored;
					sleepingParentIgnored = value;
					sleepingParentIgnoredChanged.Dispatch(this, value, previous);

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

		public bool IsSleepingParentIgnored
		{
			get
			{
				return sleepingParentIgnored > 0;
			}
		}

		public Signal<IEntity, Type> SystemTypeAdded
		{
			get
			{
				return systemTypeAdded;
			}
		}

		public Signal<IEntity, Type> SystemTypeRemoved
		{
			get
			{
				return systemTypeRemoved;
			}
		}

		public List<Type> SystemTypes
		{
			get
			{
				return new List<Type>(systemTypes);
			}
		}

		public bool HasSystemType<T>() where T : ISystem
		{
			return HasSystemType(typeof(T));
		}

		public bool HasSystemType(Type type)
		{
			return systemTypes.Contains(type);
		}

		public bool AddSystemType<T>() where T : ISystem
		{
			return AddSystemType(typeof(T));
		}

		public bool AddSystemType(Type type)
		{
			if(systemTypes == null)
			{
				systemTypes = new HashSet<Type>();
			}
			if(!systemTypes.Contains(type))
			{
				systemTypes.Add(type);
				systemTypeAdded.Dispatch(this, type);
				return true;
			}
			return false;
		}

		public bool RemoveSystemType<T>() where T : ISystem
		{
			return RemoveSystemType(typeof(T));
		}

		public bool RemoveSystemType(Type type)
		{
			if(systemTypes != null)
			{
				if(systemTypes.Contains(type))
				{
					systemTypes.Remove(type);
					systemTypeRemoved.Dispatch(this, type);
					if(systemTypes.Count <= 0)
					{
						systemTypes = null;
					}
					return true;
				}
			}
			return false;
		}

		public void RemoveSystems()
		{
			if(systemTypes != null)
			{
				foreach(Type systemType in systemTypes)
				{
					RemoveSystemType(systemType);
				}
			}
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
				}
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
				if(IsDisposedWhenUnmanaged != value)
				{
					isDisposedWhenUnmanaged = value;

					if(parent == null && value)
					{
						Dispose();
					}
				}

			}
		}

		public string Dump(string indent = "")
		{
			int index;
			string text = indent;

			index = parent != null ? parent.GetChildIndex(this) + 1 : 1;
			text += "Child " + index;
			text += "\n  " + indent;
			text += "Unique Name            = " + globalName;
			text += "\n  " + indent;
			text += "Name                   = " + localName;
			//text += "\n  " + indent;
			//text += "Num Sleep              = " + this._totalSleeping;
			//text += "\n  " + indent;
			//text += "Ignore Parent Sleeping = " + this._isSleepingParentIgnored;
			//text += "\n  " + indent;
			//text += "Destroy When Unmanaged = " + this._isDestroyedWhenUnmanaged;

			text += "\n  " + indent;
			text += "Components";
			index = 0;
			foreach(Type componentType in components.Keys)
			{
				IComponent component = components[componentType];
				text += "\n    " + indent;
				text += "Component " + (++index);
				text += "\n      " + indent;
				text += "Class                  = " + componentType.FullName;
				text += "\n      " + indent;
				text += "Instance               = " + component.GetType().FullName;
				text += "\n      " + indent;
				text += "Shareable              = " + component.IsShareable;
				text += "\n      " + indent;
				text += "Num Entities           = " + component.Entities.Count;
				text += "\n      " + indent;
				text += "Dispose When Unmanaged = " + component.IsDisposedWhenUnmanaged;
			}

			text += "\n  " + indent;
			text += "Children";
			text += "\n";
			for(index = 0; index < children.Count; ++index)
			{
				text += children[index].Dump(indent + "    ");
			}

			return text;
		}
	}
}
