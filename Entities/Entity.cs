using Atlas.Components;
using Atlas.Signals;
using Atlas.Systems;
using System;
using System.Collections.Generic;

namespace Atlas.Entities
{
	sealed class Entity
	{
		private Atlas atlas;

		private EntityManager entityManager;
		private Signal<Entity, EntityManager> entityManagerChanged = new Signal<Entity, EntityManager>();

		private string uniqueName = "";
		private Signal<Entity, string, string> uniqueNameChanged = new Signal<Entity, string, string>();

		private string name = "";
		private Signal<Entity, string, string> nameChanged = new Signal<Entity, string, string>(false);

		private List<Entity> children = new List<Entity>();
		private Signal<Entity, Entity, int> childAdded = new Signal<Entity, Entity, int>();
		private Signal<Entity, Entity, int> childRemoved = new Signal<Entity, Entity, int>();
		//Bool is true for inclusive (1 through 4) and false for exclusive (1 and 4)
		private Signal<Entity, int, int, bool> childIndicesChanged = new Signal<Entity, int, int, bool>(); //Indices of children


		private Entity parent;
		private Signal<Entity, Entity, Entity> parentChanged = new Signal<Entity, Entity, Entity>();
		private Signal<Entity, int, int> parentIndexChanged = new Signal<Entity, int, int>(); //Index within parent

		private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
		private Signal<Entity, Component, Type> componentAdded = new Signal<Entity, Component, Type>();
		private Signal<Entity, Component, Type> componentRemoved = new Signal<Entity, Component, Type>();

		private HashSet<Type> systemTypes = new HashSet<Type>();
		private Signal<Entity, Type> systemTypeAdded = new Signal<Entity, Type>();
		private Signal<Entity, Type> systemTypeRemoved = new Signal<Entity, Type>();

		private int sleeping = 0;
		private Signal<Entity, int, int> sleepingChanged = new Signal<Entity, int, int>();
		private int sleepingParentIgnored = 0;
		private Signal<Entity, int, int> sleepingParentIgnoredChanged = new Signal<Entity, int, int>();

		private bool isDisposedWhenUnmanaged = true;

		public static implicit operator bool(Entity entity)
		{
			return entity != null;
		}

		public Entity(string uniqueName = "", string name = "")
		{
			UniqueName = uniqueName;
			Name = name;
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
				RemoveChildren();
				RemoveComponents();
				Parent = null;
				Sleeping = 0;
				SleepingParentIgnored = 0;
				IsDisposedWhenUnmanaged = true;
				UniqueName = "";
				Name = "";

				entityManagerChanged.Dispose();
				uniqueNameChanged.Dispose();
				nameChanged.Dispose();
				childAdded.Dispose();
				childRemoved.Dispose();
				parentChanged.Dispose();
				componentAdded.Dispose();
				componentRemoved.Dispose();
				sleepingChanged.Dispose();
				sleepingParentIgnoredChanged.Dispose();
			}
		}

		public string UniqueName
		{
			get
			{
				return uniqueName;
			}
			set
			{

				if(uniqueName != value && !string.IsNullOrWhiteSpace(value))
				{
					if(entityManager != null && !entityManager.IsUniqueName(value))
					{
						return;
					}
					string previous = uniqueName;
					uniqueName = value;
					uniqueNameChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<Entity, string, string> UniqueNameChanged
		{
			get
			{
				return uniqueNameChanged;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if(name != value && value != null)
				{
					string previous = name;
					name = value;
					nameChanged.Dispatch(this, value, previous);
				}
			}
		}

		public Signal<Entity, string, string> NameChanged
		{
			get
			{
				return nameChanged;
			}
		}

		public Atlas Atlas
		{
			get
			{
				return atlas;
			}
			internal set
			{
				if(atlas != value)
				{
					Atlas previous = atlas;
					atlas = value;
				}
			}
		}

		public EntityManager EntityManager
		{
			get
			{
				return entityManager;
			}
			internal set
			{
				if(entityManager != value)
				{
					EntityManager previous = entityManager;
					entityManager = value;
					entityManagerChanged.Dispatch(this, previous);
				}
			}
		}

		public Signal<Entity, EntityManager> EntityManagerChanged
		{
			get
			{
				return entityManagerChanged;
			}
		}

		public Entity Root
		{
			get
			{
				return entityManager != null ? entityManager.ComponentManager : null;
			}
		}

		public bool HasComponent<T>() where T : Component
		{
			return HasComponent(typeof(T));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		public Component GetComponent<T>() where T : Component
		{
			return GetComponent(typeof(T));
		}

		public Component GetComponent(Type componentType)
		{
			return components.ContainsKey(componentType) ? components[componentType] : null;
		}

		public Type GetComponentType(Component component)
		{
			if(component != null)
			{
				foreach(Type type in components.Keys)
				{
					if(components[type] == component)
					{
						return type;
					}
				}
			}
			return null;
		}

		public List<Component> Components
		{
			get
			{
				return new List<Component>(components.Values);
			}
		}

		public List<Type> ComponentTypes
		{
			get
			{
				return new List<Type>(components.Keys);
			}
		}

		public Component AddComponent(Component component)
		{
			return AddComponent(component, null);
		}

		public Component AddComponent(Component component, Type componentType)
		{
			return AddComponent(component, componentType, int.MaxValue);
		}

		public Component AddComponent(Component component, int index)
		{
			return AddComponent(component, null, index);
		}

		public Component AddComponent(Component component, Type componentType = null, int index = int.MaxValue)
		{
			if(component != null)
			{
				if(!component.IsShareable && component.NumComponentManagers > 0)
				{
					return null;
				}
				if(componentType == null)
				{
					componentType = component.GetType();
				}
				else if(!componentType.IsInstanceOfType(component))
				{
					return null;
				}
				if(!components.ContainsKey(componentType) || components[componentType] != component)
				{
					RemoveComponent(componentType);
					components.Add(componentType, component);
					component.AddComponentManager(this, componentType, index);
					componentAdded.Dispatch(this, component, componentType);
				}
			}
			return component;
		}

		public Signal<Entity, Component, Type> ComponentAdded
		{
			get
			{
				return componentAdded;
			}
		}

		public Component RemoveComponent(Type componentType)
		{
			if(componentType != null)
			{
				if(components.ContainsKey(componentType))
				{
					Component component = components[componentType];
					components.Remove(componentType);
					component.RemoveComponentManager(this);
					componentRemoved.Dispatch(this, component, componentType);
					return component;
				}
			}
			return null;
		}

		public Signal<Entity, Component, Type> ComponentRemoved
		{
			get
			{
				return componentRemoved;
			}
		}

		public Component RemoveComponent(Component component)
		{
			return RemoveComponent(GetComponentType(component));
		}

		public void RemoveComponents()
		{
			foreach(Type componentType in components.Keys)
			{
				RemoveComponent(componentType);
			}
		}

		public Entity Parent
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

		public bool Contains(Entity entity)
		{
			while(entity != this && entity != null)
			{
				entity = entity.Parent;
			}
			return entity == this;
		}

		public bool HasChild(Entity child)
		{
			return children.Contains(child);
		}

		public bool HasChild(string name)
		{
			for(int index = 0; index < children.Count; ++index)
			{
				if(children[index].Name == name)
				{
					return true;
				}
			}
			return false;
		}

		public Entity AddChild(Entity child)
		{
			return AddChild(child, children.Count);
		}

		public Entity AddChild(Entity child, int index)
		{
			if(child != null)
			{
				if(child.Parent == this)
				{
					if(!HasChild(child))
					{
						children.Insert(index, child);
						if(IsSleeping && !child.IsSleepingParentIgnored)
						{
							++child.Sleeping;
						}
						childAdded.Dispatch(this, child, index);
					}
					else
					{
						SetChildIndex(child, index);
					}
				}
				else
				{
					if(child.Contains(this))
					{
						return null;
					}
					child.SetParent(this, index);
				}
				return child;
			}
			return null;
		}

		/*
		 * == Remove
		 * ParentOld.ChildRemoved
		 * ParentOld.IndicesChanged
		 * 
		 * SiblingsOld.IndexChanged
		 * 
		 * Child.ParentChanged
		 * Child.IndexChanged
		 * 
		 * == Remove + Add
		 * ParentOld.ChildRemoved
		 * ParentOld.IndicesChanged
		 * 
		 * SiblingsOld.IndexChanged
		 * 
		 * ParentNew.ChildAdded
		 * ParentNew.IndicesChanged
		 * 
		 * SiblingsNew.IndexChanged
		 * 
		 * Child.ParentChanged
		 * Child.IndexChanged
		 */
		public Entity AddChild(Entity child, int index, bool b)
		{
			if(child.parent != this)
			{
				Entity previousParent = null;
				int previousIndex = -1;
				if(child.parent != null)
				{
					previousParent = child.parent;
					previousIndex = RemoveAChild(child);
				}

				//Next Parent hierarchy changes
				child.parent = this;
				children.Insert(index, child);
				childAdded.Dispatch(this, child, index);

				int numChildren = children.Count;

				//Next Parent index changes
				childIndicesChanged.Dispatch(this, index, numChildren, true);

				//sibling index changes
				for(int i = index; i < numChildren; ++i)
				{
					Entity sibling = children[i];
					sibling.parentIndexChanged.Dispatch(sibling, i + 1, i);
				}

				//Child index changes
				if(index != previousIndex)
				{
					child.parentIndexChanged.Dispatch(child, index, previousIndex);
				}

				//Child hierachy changes
				child.parentChanged.Dispatch(child, this, previousParent);

			}
			return null;
		}

		public Signal<Entity, Entity, int> ChildAdded
		{
			get
			{
				return childAdded;
			}
		}

		public Entity RemoveChild(Entity child)
		{
			if(child != null)
			{
				if(child.Parent != this)
				{
					if(HasChild(child))
					{

						int index = children.IndexOf(child);
						children.RemoveAt(index);
						childRemoved.Dispatch(this, child, index);
						childIndicesChanged.Dispatch(this, index, children.Count, true);
						for(int i = index; i < children.Count; ++i)
						{
							Entity sibling = children[i];
							sibling.parentIndexChanged.Dispatch(sibling, i, i + 1);
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
					SetParent();
					return child;
				}
			}
			return null;
		}

		private int RemoveAChild(Entity child)
		{
			int index = children.IndexOf(child);
			//Previous Parent hierarchy changes
			childRemoved.Dispatch(this, child, index);
			children.Remove(child);
			child.parent = null;

			//Previous Parent index changes
			childIndicesChanged.Dispatch(this, index, children.Count, true);

			//Previous sibling index changes
			for(int i = index; i < children.Count; ++i)
			{
				Entity sibling = children[i];
				sibling.parentIndexChanged.Dispatch(sibling, i, i + 1);
			}
			return index;
		}

		public Entity RemoveChild(Entity child, bool b)
		{
			if(child.parent == this)
			{
				int index = RemoveAChild(child);

				//Child hierarchy changes
				child.parentChanged.Dispatch(child, null, this);

				//Child index changes
				child.parentIndexChanged.Dispatch(child, -1, index);

			}
			return null;
		}

		private void RChild(Entity child)
		{
			if(child.parent == this)
			{
				int previousIndex = children.IndexOf(child);
				children.RemoveAt(previousIndex);
				child.parent = null;

				List<Entity> siblings = children.GetRange(previousIndex, children.Count - previousIndex);
				int childCount = children.Count;

				childRemoved.Dispatch(this, child, previousIndex);
				childIndicesChanged.Dispatch(this, previousIndex, childCount - 1, true);

				for(int si = 0; si < siblings.Count; ++si)
				{
					Entity sibling = siblings[si];
					int sibIndex = previousIndex + si;
					sibling.parentIndexChanged.Dispatch(sibling, sibIndex, sibIndex + 1);
				}

				child.parentChanged.Dispatch(child, null, this);
				child.parentIndexChanged.Dispatch(child, -1, previousIndex);
			}
		}

		private void AChild(Entity child, int index)
		{
			if(child.parent != this)
			{
				Entity previousParent = null;
				int previousIndex = -1;

				if(child.parent != null)
				{
					previousParent = child.parent;
					previousIndex = previousParent.children.IndexOf(child);
					previousParent.children.RemoveAt(previousIndex);
				}

				child.parent = this;
				child.parent.children.Insert(index, child);

			}
		}

		public Signal<Entity, Entity, int> ChildRemoved
		{
			get
			{
				return childRemoved;
			}
		}

		public Entity RemoveChild(int index)
		{
			if(index < 0)
				return null;
			if(index > children.Count - 1)
				return null;
			return RemoveChild(children[index]);
		}

		public void RemoveChildren()
		{
			while(children.Count > 0)
			{
				children[children.Count - 1].Dispose();
			}
		}

		private void SetParent(Entity next = null, int index = int.MaxValue)
		{
			if(parent != next)
			{
				Entity previous = parent;
				parent = next;
				if(previous != null)
				{
					previous.RemoveChild(this);
				}
				if(next != null)
				{
					next.AddChild(this, index);
				}
				parentChanged.Dispatch(this, next, previous);

				if(!next && isDisposedWhenUnmanaged)
				{
					Dispose();
				}
			}
		}

		public Signal<Entity, Entity, Entity> ParentChanged
		{
			get
			{
				return parentChanged;
			}
		}

		public Entity GetEntityByNames(string hierarchy)
		{
			if(name != null)
			{
				string[] names = hierarchy.Split('/');
				Entity entity = this;

				foreach(string name in names)
				{
					if(name == "..")
					{
						entity = entity.parent;
					}
					else
					{
						entity = entity.GetChild(name);
					}

					if(entity == null)
					{
						break;
					}
				}
				return entity;
			}
			return null;
		}

		public Entity GetChild(string name)
		{
			if(name != null)
			{
				foreach(Entity child in children)
				{
					if(child.name == name)
					{
						return child;
					}
				}
			}
			return null;
		}

		public Entity GetChild(int index)
		{
			if(index < 0)
				return null;
			if(index > children.Count - 1)
				return null;
			return children[index];
		}

		public int GetChildIndex(Entity child)
		{
			return children.IndexOf(child);
		}

		public bool SetChildIndex(Entity child, int index)
		{
			int previous = children.IndexOf(child);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			int next = index;

			children.RemoveAt(previous);
			children.Insert(next, child);

			child.parentIndexChanged.Dispatch(child, next, previous);
			if(next > previous)
			{
				//Children shift down 0<-[1]
				for(index = previous; index < next; ++index)
				{
					child = children[index];
					child.parentIndexChanged.Dispatch(child, index, index + 1);
				}

				childIndicesChanged.Dispatch(this, previous, next, true);
			}
			else
			{
				//Children shift up [0]->1
				for(index = previous; index > next; --index)
				{
					child = children[index];
					child.parentIndexChanged.Dispatch(child, index, index - 1);
				}
				childIndicesChanged.Dispatch(this, next, previous, true);
			}
			return true;
		}

		public Signal<Entity, int, int> ParentIndexChanged
		{
			get
			{
				return parentIndexChanged;
			}
		}

		public bool SwapChildren(Entity child1, Entity child2)
		{
			if(child1 == null)
				return false;
			if(child2 == null)
				return false;
			int index1 = children.IndexOf(child1);
			int index2 = children.IndexOf(child2);
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
			Entity child1 = children[index1];
			Entity child2 = children[index2];
			children[index1] = child2;
			children[index2] = child1;
			child1.parentIndexChanged.Dispatch(child1, index2, index1);
			child2.parentIndexChanged.Dispatch(child2, index1, index2);
			childIndicesChanged.Dispatch(this, Math.Min(index1, index2), Math.Max(index1, index2), false);
			return true;
		}

		public List<Entity> Children
		{
			get
			{
				return new List<Entity>(children);
			}
		}

		public int NumChildren
		{
			get
			{
				return children.Count;
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
						foreach(Entity child in children)
						{
							if(!child.IsSleepingParentIgnored)
							{
								++child.Sleeping;
							}
						}

					}
					else if(value <= 0 && previous > 0)
					{
						foreach(Entity child in children)
						{
							if(!child.IsSleepingParentIgnored)
							{
								--child.Sleeping;
							}
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

		public Signal<Entity, Type> SystemTypeAdded
		{
			get
			{
				return systemTypeAdded;
			}
		}

		public Signal<Entity, Type> SystemTypeRemoved
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

		public bool HasSystemType<T>()
		{
			return HasSystemType(typeof(T));
		}

		public bool HasSystemType(Type systemType)
		{
			return systemTypes.Contains(systemType);
		}

		public bool AddSystemType<T>() where T : AtlasSystem
		{
			return AddSystemType(typeof(T));
		}

		public bool AddSystemType(Type systemType)
		{
			if(typeof(AtlasSystem).IsAssignableFrom(systemType))
			{
				if(!systemTypes.Contains(systemType))
				{
					systemTypes.Add(systemType);
					systemTypeAdded.Dispatch(this, systemType);
					return true;
				}
			}
			return false;
		}

		public bool RemoveSystemType<T>() where T : AtlasSystem
		{
			return RemoveSystemType(typeof(T));
		}

		public bool RemoveSystemType(Type systemType)
		{
			if(systemTypes.Contains(systemType))
			{
				systemTypes.Remove(systemType);
				systemTypeRemoved.Dispatch(this, systemType);
				return true;
			}
			return false;
		}

		public void RemoveSystems()
		{
			foreach(Type systemType in systemTypes)
			{
				RemoveSystemType(systemType);
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

		public string Dump(string indent = "")
		{
			int index;
			string text = indent;

			index = parent != null ? parent.GetChildIndex(this) + 1 : 1;
			text += "Child " + index;
			text += "\n  " + indent;
			text += "Unique Name            = " + uniqueName;
			text += "\n  " + indent;
			text += "Name                   = " + name;
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
				Component component = components[componentType];
				text += "\n    " + indent;
				text += "Component " + (++index);
				text += "\n      " + indent;
				text += "Class                  = " + componentType.FullName;
				text += "\n      " + indent;
				text += "Instance               = " + component.GetType().FullName;
				text += "\n      " + indent;
				text += "Shareable              = " + component.IsShareable;
				text += "\n      " + indent;
				text += "Num Entities           = " + component.NumComponentManagers;
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
