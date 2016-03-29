using Atlas.Components;
using Atlas.Signals;
using System;
using System.Collections.Generic;

namespace Atlas.Entities
{
    class Entity
    {
        private EntityManager entityManager;
        private Signal<Entity, EntityManager> entityManagerChanged = new Signal<Entity, EntityManager>();

        private string uniqueName = "";
        private Signal<Entity, string> uniqueNameChanged = new Signal<Entity, string>();

        private string name = "";
        private Signal<Entity, string> nameChanged = new Signal<Entity, string>();
        
        private List<Entity> children = new List<Entity>();
        private Signal<Entity, Entity> childAdded = new Signal<Entity, Entity>();
        private Signal<Entity, Entity> childRemoved = new Signal<Entity, Entity>();

        private Entity parent;
        private Signal<Entity, Entity> parentChanged = new Signal<Entity, Entity>();

        private Dictionary<Type, Component> components = new Dictionary<Type, Component>();
        private Signal<Entity, Type> componentAdded = new Signal<Entity, Type>();
        private Signal<Entity, Type> componentRemoved = new Signal<Entity, Type>();

        private bool isDisposedWhenUnmanaged = true;
        
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
                parent = null;
            }
            else
            {
                RemoveChildren();
                RemoveComponents();
                parent = null;
                //TotalSleeping = 0;
                //IsSleepingParentIgnored = false;
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
                if(value != null && name != value)
                {
                    if(entityManager != null && !entityManager.IsUniqueName(value))
                    {
                        return;
                    }
                    string previous = uniqueName;
                    uniqueName = value;
                    uniqueNameChanged.Dispatch(this, previous);
                }
            }
        }

        public Signal<Entity, string> UniqueNameChanged
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
                if(value != null && name != value)
                {
                    string previous = name;
                    name = value;
                    nameChanged.Dispatch(this, previous);
                }
            }
        }

        public Signal<Entity, string> NameChanged
        {
            get
            {
                return nameChanged;
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
                if(entityManager != null)
                {
                    return entityManager.ComponentManager;
                }
                return null;
            }
        }

        public bool HasComponent<T>()
        {
            return HasComponent(typeof(T));
        }

        public bool HasComponent(Type type)
        {
            return components.ContainsKey(type);
        }

        public Component GetComponent<T>()
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
                    componentAdded.Dispatch(this, componentType);
                }
            }
		    return component;
	    }

        public Signal<Entity, Type> ComponentAdded
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
				    componentRemoved.Dispatch(this, componentType);
                    Component component = components[componentType];
				    components.Remove(componentType);
				    component.RemoveComponentManager(this);
                    return component;
			    }
            }
		    return null;
	    }

        public Signal<Entity, Type> ComponentRemoved
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

        public bool ContainsEntity(Entity entity)
        {
            if(entity != null)
            {
                do
                {
                    if(this == entity)
                    {
                        return true;
                    }
                    entity = entity.Parent;
                }
                while(entity != null);
            }
            return false;
        }
        
        public bool HasChild(Entity child)
        {
            return children.Contains(child);
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

                        /*if (this._totalSleeping > 0 && !child._isSleepingParentIgnored)
                        {
                            ++child.totalSleeping;
                        }*/

                        childAdded.Dispatch(this, child);
                    }
                    else
                    {
                        SetChildIndex(child, index);
                    }
                }
                else
                {
                    if(child.ContainsEntity(this))
                    {
                        return null;
                    }
                    child.SetParent(this, index);
                }
                return child;
            }
            return null;
        }

        public Signal<Entity, Entity> ChildAdded
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
                        childRemoved.Dispatch(this, child);
                        children.Remove(child);
                        /*if(this._totalSleeping > 0 && !child._isSleepingParentIgnored)
                        {
                            --child.totalSleeping;
                        }*/
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

        public Signal<Entity, Entity> ChildRemoved
        {
            get
            {
                return childRemoved;
            }
        }

        public Entity RemoveChild(int index)
        {
            if(index < 0) return null;
            if(index > children.Count - 1) return null;
            return RemoveChild(children[index]);
        }

        public void RemoveChildren()
        {
            while(children.Count > 0)
            {
                children[children.Count - 1].Dispose();
            }
        }

        private void SetParent(Entity value = null, int index = int.MaxValue)
	    {
		    if(parent != value)
		    {
			    Entity previous = parent;
			    parent = value;
			    if(previous != null)
			    {
				    previous.RemoveChild(this);
			    }
			    if(value != null)
			    {
				    value.AddChild(this, index);
                }
                parentChanged.Dispatch(this, previous);
            }
        }

        public Signal<Entity, Entity> ParentChanged
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
					    entity = entity.Parent;
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
                    if(child.Name == name)
                    {
                        return child;
                    }
                }
		    }
		    return null;
	    }

        public Entity GetChild(int index)
        {
            if(index < 0) return null;
            if(index > children.Count - 1) return null;
            return children[index];
        }

        public int GetChildIndex(Entity child)
	    {
            return children.IndexOf(child);
	    }

        public bool SetChildIndex(Entity child, int index)
	    {
		    int previousIndex = children.IndexOf(child);
		
		    if(previousIndex< 0) return false;
		
		    int numChildren = children.Count;
		    if(index <= 0)
		    {
			    index = 0;
		    }
		    else if(index > numChildren - 1)
		    {
			    index = numChildren - 1;
		    }
		
		    if(previousIndex == index) return true;
		
		    //-1 is needed since technically you lost a child on the previous splice.
		    if(index > previousIndex)
		    {
			    --index;
		    }

            children.RemoveAt(previousIndex);
		    children.Insert(index, child);
		    return true;
        }

        public bool SwapChildren(Entity child1, Entity child2)
	    {
		    if(child1 == null) return false;
		    if(child2 == null) return false;
		    int index1 = children.IndexOf(child1);
		    int index2 = children.IndexOf(child2);
		    return SwapChildren(index1, index2);
        }

        public bool SwapChildren(int index1, int index2)
        {
            if (index1 < 0) return false;
            if (index2 < 0) return false;
            int numChildren = children.Count;
            if (index1 > numChildren - 1) return false;
            if (index2 > numChildren - 1) return false;
            Entity child = children[index1];
            children[index1] = children[index2];
            children[index2] = child;
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
