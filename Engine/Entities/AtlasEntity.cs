﻿using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Messages;
using Atlas.Engine.Systems;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Atlas.Engine.Entities
{
	public sealed class AtlasEntity : EngineObject, IEntity
	{
		#region Static Singleton

		private static AtlasEntity instance;

		/// <summary>
		/// Creates a singleton instance of the root Entity. The root Entity
		/// gets its Entity.Root value set to itself through reflection. Only
		/// one root should exist at a time. Once the root Entity is destroyed,
		/// this singleton will be null again.
		/// </summary>
		public static AtlasEntity Instance
		{
			get
			{
				if(!instance)
				{
					instance = new AtlasEntity("Root", "Root");
					Type type = instance.GetType();
					BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
					FieldInfo field = type.GetField("root", flags);
					field.SetValue(instance, instance);
				}
				return instance;
			}
		}

		public static string UniqueName
		{
			get { return Guid.NewGuid().ToString("N"); }
		}

		#endregion

		private string globalName = UniqueName;
		private string localName = UniqueName;
		private int sleeping = 0;
		private int freeSleeping = 0;
		private IEntity root;
		private IEntity parent;
		private int parentIndex = -1;
		private EngineList<IEntity> children = new EngineList<IEntity>();
		private Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		private HashSet<Type> systems = new HashSet<Type>();
		private bool autoDestroy = true;

		public AtlasEntity()
		{

		}

		public AtlasEntity(string globalName = "", string localName = "")
		{
			GlobalName = globalName;
			LocalName = localName;
		}

		protected override void Destroying()
		{
			RemoveChildren();
			RemoveComponents();
			RemoveSystems();
			GlobalName = UniqueName;
			LocalName = UniqueName;
			AutoDestroy = true;
			Parent = null;
			Sleeping = 0;
			FreeSleeping = 0;
			Root = null;

			//If this is the root Entity, then we
			//should allow another to be instantiated.
			if(instance == this)
				instance = null;

			base.Destroying();
		}

		#region Entity

		public string GlobalName
		{
			get { return globalName; }
			set
			{
				if(string.IsNullOrWhiteSpace(value))
					return;
				if(globalName == value)
					return;
				if(Engine != null && Engine.HasEntity(value))
					return;
				string previous = globalName;
				globalName = value;
				Message<IGlobalNameMessage>(new GlobalNameMessage(value, previous));
			}
		}

		public string LocalName
		{
			get { return localName; }
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
				Message<ILocalNameMessage>(new LocalNameMessage(value, previous));
			}
		}

		public string AncestorsToString(int depth = -1, bool localNames = true, string indent = "")
		{
			StringBuilder text = new StringBuilder();
			if(Parent != null && depth != 0)
			{
				text.Append(Parent.AncestorsToString(depth - 1, localNames, indent));
				var parent = Parent;
				while(parent != null && depth != 0)
				{
					text.Append("  ");
					--depth;
					parent = parent.Parent;
				}
			}
			text.Append(indent);
			text.AppendLine(localNames ? localName : globalName);
			return text.ToString();
		}

		public string DescendantsToString(int depth = -1, bool localNames = true, string indent = "")
		{
			StringBuilder text = new StringBuilder();
			text.Append(indent);
			text.AppendLine(localNames ? localName : globalName);
			if(depth != 0)
			{
				foreach(var child in children)
					text.Append(child.DescendantsToString(depth - 1, localNames, indent + "  "));
			}
			return text.ToString();
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
			return children.Find((IEntity entity) => entity.LocalName == localName);
		}

		#endregion

		#region Engine

		sealed override public IEngine Engine
		{
			get
			{
				return base.Engine;
			}
			set
			{
				if(value != null)
				{
					if(Engine == null && value.HasEntity(this))
					{
						base.Engine = value;
					}
				}
				else
				{
					if(Engine != null && !Engine.HasEntity(this))
					{
						base.Engine = value;
					}
				}
			}
		}

		#endregion

		#region Components

		public bool HasComponent<TIComponent>()
			where TIComponent : IComponent
		{
			return HasComponent(typeof(TIComponent));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		public TComponent GetComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent
		{
			return (TComponent)GetComponent(typeof(TIComponent));
		}

		public TIComponent GetComponent<TIComponent>()
			where TIComponent : IComponent
		{
			return (TIComponent)GetComponent(typeof(TIComponent));
		}

		public IComponent GetComponent(Type type)
		{
			return components.ContainsKey(type) ? components[type] : null;
		}

		public Type GetComponentType(IComponent component)
		{
			if(component == null)
				return null;
			foreach(var type in components.Keys)
			{
				if(components[type] == component)
					return type;
			}
			return null;
		}

		public List<Type> GetComponentTypes(IComponent component)
		{
			var types = new List<Type>();
			if(component == null)
				return types;
			foreach(var type in components.Keys)
			{
				if(components[type] == component)
					types.Add(type);
			}
			return types;
		}

		public IReadOnlyDictionary<Type, IComponent> Components
		{
			get { return components; }
		}

		//New component with Type
		public TComponent AddComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent, new()
		{
			return (TComponent)AddComponent(new TComponent(), typeof(TIComponent), 0);
		}

		//Component with Type
		public TComponent AddComponent<TIComponent, TComponent>(TComponent component)
			where TIComponent : IComponent
			where TComponent : TIComponent
		{
			return (TComponent)AddComponent(component, typeof(TIComponent), int.MaxValue);
		}

		//Component with Type, index
		public TComponent AddComponent<TIComponent, TComponent>(TComponent component, int index)
			where TIComponent : IComponent
			where TComponent : TIComponent
		{
			return (TComponent)AddComponent(component, typeof(TIComponent), index);
		}

		//New Component
		public TComponent AddComponent<TComponent>()
			where TComponent : IComponent, new()
		{
			return (TComponent)AddComponent(new TComponent(), null, 0);
		}

		//Component
		public TIComponent AddComponent<TIComponent>(IComponent component)
			where TIComponent : IComponent
		{
			return (TIComponent)AddComponent(component, typeof(TIComponent), int.MaxValue);
		}

		//Component, index
		public TIComponent AddComponent<TIComponent>(IComponent component, int index)
			where TIComponent : IComponent
		{
			return (TIComponent)AddComponent(component, typeof(TIComponent), index);
		}

		public IComponent AddComponent(IComponent component)
		{
			return AddComponent(component, null, int.MaxValue);
		}

		public IComponent AddComponent(IComponent component, Type type)
		{
			return AddComponent(component, type, int.MaxValue);
		}

		public IComponent AddComponent(IComponent component, int index)
		{
			return AddComponent(component, null, index);
		}

		public IComponent AddComponent(IComponent component, Type type, int index)
		{
			if(component == null)
				return null;
			//The component isn't shareable and it already has a manager.
			if(component.Manager != null || component.Managers.Contains(this))
				return null;
			if(type == null)
				type = component.GetType();
			else
			{
				if(type == typeof(IComponent))
					return null;
				if(!type.IsInstanceOfType(component))
					return null;
			}
			if(!components.ContainsKey(type) || components[type] != component)
			{
				//Entity is no longer considered destroyed if it's adding Components.
				Construct();
				RemoveComponent(type);
				components.Add(type, component);
				component.AddManager(this, type, index);
				Message<IComponentAddMessage>(new ComponentAddMessage(type, component));
			}
			return component;
		}

		public TComponent RemoveComponent<TIComponent, TComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent
		{
			return (TComponent)RemoveComponent(typeof(TIComponent));
		}

		public TIComponent RemoveComponent<TIComponent>()
			where TIComponent : IComponent
		{
			return (TIComponent)RemoveComponent(typeof(TIComponent));
		}

		public IComponent RemoveComponent(Type type)
		{
			if(type == null)
				return null;
			if(!components.ContainsKey(type))
				return null;
			IComponent component = components[type];
			Message<IComponentRemoveMessage>(new ComponentRemoveMessage(type, component));
			components.Remove(type);
			component.RemoveManager(this, type);
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
			foreach(var type in new List<Type>(components.Keys))
			{
				RemoveComponent(type);
			}
			return true;
		}

		#endregion

		#region Hierarchy

		public IEntity Root
		{
			get { return root; }
			private set
			{
				if(root == value)
					return;
				var previous = root;
				root = value;
				Message<IRootMessage>(new RootMessage(value, previous));
			}
		}

		public bool HasChild(string localName)
		{
			return GetChild(localName) != null;
		}

		public bool HasChild(IEntity child)
		{
			return children.Contains(child);
		}

		private IEntity GetEntity(string globalName, string localName)
		{
			return Engine != null ? Engine.GetEntity(true, globalName, localName) : new AtlasEntity(globalName, localName);
		}

		public IEntity AddChild(string globalName = "", string localName = "")
		{
			return AddChild(GetEntity(globalName, localName), children.Count);
		}

		public IEntity AddChild(int index, string globalName = "", string localName = "")
		{
			return AddChild(GetEntity(globalName, localName), index);
		}

		public bool AddChildren(int index, params IEntity[] children)
		{
			bool success = true;
			foreach(var child in children)
			{
				if(AddChild(child, index++) == null)
					success = false;
			}
			return success;
		}

		public bool AddChildren(params IEntity[] children)
		{
			return AddChildren(this.children.Count, children);
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
				if(!HasChild(child))
				{
					if(HasChild(child.LocalName))
						child.LocalName = UniqueName;
					Construct();
					children.Insert(index, child);
					Message<IChildAddMessage>(new ChildAddMessage(index, child));
					Message<IChildrenMessage>(new ChildrenMessage());
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

		public IEntity RemoveChild(IEntity child)
		{
			if(child == null)
				return null;
			if(child.Parent != this)
			{
				if(!HasChild(child))
					return null;
				int index = children.IndexOf(child);
				Message<IChildRemoveMessage>(new ChildRemoveMessage(index, child));
				Message<IChildrenMessage>(new ChildrenMessage());
				//Could've been readded suring messaging?
				if(child.Parent != this)
					children.Remove(child);
			}
			else
			{
				child.SetParent(null, -1);
			}
			return child;
		}

		public IEntity RemoveChild(int index)
		{
			return RemoveChild(children[index]);
		}

		public IEntity RemoveChild(string localName)
		{
			return RemoveChild(GetChild(localName));
		}

		public bool RemoveChildren()
		{
			if(children.Count <= 0)
				return false;
			foreach(var child in children.Backward())
				child.Destroy();
			return true;
		}

		public IEntity Parent
		{
			get { return parent; }
			set { SetParent(value); }
		}

		public bool SetParent(IEntity next = null, int index = int.MaxValue)
		{
			if(this == Root)
				return false;
			if(parent == next)
				return false;
			//Can't set a descendant of this as a parent.
			if(HasDescendant(next))
				return false;
			Construct();
			IEntity previous = parent;
			parent = next;
			Message<IParentMessage>(new ParentMessage(next, previous));

			int sleeping = 0;
			//Extra previous and next checks against parent
			//in case an event changes the parent mid dispatch.
			if(previous != null && parent != previous)
			{
				previous.RemoveChild(this);
				if(!IsFreeSleeping && previous.IsSleeping)
					--sleeping;
			}
			if(next != null && parent == next)
			{
				index = Math.Max(0, Math.Min(index, next.Children.Count));
				next.AddChild(this, index);
				if(!IsFreeSleeping && next.IsSleeping)
					++sleeping;
			}
			//If parent becomes null, this won't get sent to anyone below...
			//...Which might really be intended/expected behavior.
			//Might still need to listen for parent changes in AtlasEngine.
			//Message<IParentMessage>(new ParentMessage(next, previous));

			SetParentIndex(index);
			Sleeping += sleeping;
			Root = next?.Root;
			if(AutoDestroy && parent == null)
				Destroy();
			return true;
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set
			{
				parent?.SetChildIndex(this, value);
			}
		}

		public bool HasDescendant(IEntity descendant)
		{
			while(descendant != null && descendant != this)
				descendant = descendant.Parent;
			return descendant == this;
		}

		public bool HasAncestor(IEntity ancestor)
		{
			return ancestor != null ? ancestor.HasDescendant(this) : false;
		}

		public bool HasSibling(IEntity sibling)
		{
			return Parent != null ? Parent.HasChild(sibling) : false;
		}

		public IEntity GetChild(int index)
		{
			return children[index];
		}

		public int GetChildIndex(IEntity child)
		{
			return children.IndexOf(child);
		}

		public bool SetChildIndex(IEntity child, int index)
		{
			int previous = children.IndexOf(child);

			if(previous == index)
				return true;
			if(previous < 0)
				return false;

			index = Math.Max(0, Math.Min(index, children.Count - 1));

			children.RemoveAt(previous);
			children.Insert(index, child);
			Message<IChildrenMessage>(new ChildrenMessage());
			return true;
		}

		public bool SwapChildren(IEntity child1, IEntity child2)
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
			if(!children.Swap(index1, index2))
				return false;
			Message<IChildrenMessage>(new ChildrenMessage());
			return true;
		}

		public IReadOnlyEngineList<IEntity> Children
		{
			get { return children; }
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			Message<IParentIndexMessage>(new ParentIndexMessage(value, previous));
		}

		#endregion

		#region Sleep

		public int Sleeping
		{
			get { return sleeping; }
			set
			{
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				Message<ISleepMessage>(new SleepMessage(value, previous));
			}
		}

		public bool IsSleeping
		{
			get { return sleeping > 0; }
		}

		public int FreeSleeping
		{
			get { return freeSleeping; }
			set
			{
				if(freeSleeping == value)
					return;
				int previous = freeSleeping;
				freeSleeping = value;
				Message<IFreeSleepMessage>(new FreeSleepMessage(value, previous));
				if(parent == null)
					return;
				if(value > 0 && previous <= 0)
				{
					if(parent.IsSleeping)
						--Sleeping;
				}
				else if(value <= 0 && previous > 0)
				{
					if(parent.IsSleeping)
						++Sleeping;
				}
			}
		}

		public bool IsFreeSleeping
		{
			get { return freeSleeping > 0; }
		}

		#endregion

		#region Systems

		public IReadOnlyCollection<Type> Systems
		{
			get { return (IReadOnlyCollection<Type>)systems; }
		}

		public bool HasSystem<TSystem>() where TSystem : ISystem
		{
			return HasSystem(typeof(TSystem));
		}

		public bool HasSystem(Type type)
		{
			return systems.Contains(type);
		}

		public bool AddSystem<TSystem>() where TSystem : ISystem
		{
			return AddSystem(typeof(TSystem));
		}

		public bool AddSystem(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(type == typeof(ISystem))
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(systems.Contains(type))
				return false;
			Construct();
			systems.Add(type);
			Message<ISystemTypeAddMessage>(new SystemTypeAddMessage(type));
			return true;
		}

		public bool RemoveSystem<TSystem>() where TSystem : ISystem
		{
			return RemoveSystem(typeof(TSystem));
		}

		public bool RemoveSystem(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface)
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type))
				return false;
			if(!systems.Contains(type))
				return false;
			systems.Remove(type);
			Message<ISystemTypeRemoveMessage>(new SystemTypeRemoveMessage(type));
			return true;
		}

		public bool RemoveSystems()
		{
			if(systems.Count <= 0)
				return false;
			foreach(var system in new List<Type>(systems))
				RemoveSystem(system);
			return true;
		}

		#endregion

		public bool AutoDestroy
		{
			get { return autoDestroy; }
			set
			{
				if(autoDestroy == value)
					return;
				var previous = autoDestroy;
				autoDestroy = value;
				Message<IAutoDestroyMessage>(new AutoDestroyMessage(value, previous));
			}
		}

		sealed override public void Message<TMessage>(TMessage message)
		{
			//Keep track of what child told the parent to Dispatch().
			var previousTarget = message.CurrentMessenger;

			//Standard Message() call.
			//Set Target if null and Dispatch() event by type;
			base.Message(message);

			//Send Message to children.
			foreach(var child in children)
			{
				//Don't send Message back to child that told the parent to Message().
				if(child == previousTarget)
					continue;
				child.Message(message);
				//Reset CurrentTarget back to this so
				//the next child (and parent) can block messaging from its original source.
				((IMessageBase)message).CurrentMessenger = this;
			}

			//Send Message to parent.
			//Don't send Message back to parent that told the child to Message().
			if(Parent != null && Parent != previousTarget)
				Parent.Message(message);
		}

		protected override void Messaging(IMessage message)
		{
			if(message is ISleepMessage)
			{
				if(message.Messenger != Parent)
					return;
				if(IsFreeSleeping)
					return;
				var cast = message as ISleepMessage;
				if(cast.CurrentValue > 0 && cast.PreviousValue <= 0)
					++Sleeping;
				else if(cast.CurrentValue <= 0 && cast.PreviousValue > 0)
					--Sleeping;
			}
			else if(message is IRootMessage)
			{
				if(message.Messenger != Parent)
					return;
				var cast = message as IRootMessage;
				Root = cast.Messenger.Root;
			}
			else if(message is IChildrenMessage)
			{
				if(message.Messenger != Parent)
					return;
				SetParentIndex(Parent.GetChildIndex(this));
			}
			else if(message is IAutoDestroyMessage)
			{
				if(!message.AtMessenger)
					return;
				var cast = message as IAutoDestroyMessage;
				if(cast.CurrentValue && parent == null)
					Destroy();
			}
			base.Messaging(message);
		}

		public override string ToString()
		{
			return GlobalName;
		}

		public string ToInfoString(int depth, bool addComponents, bool addEntities, bool addSystems, string indent = "")
		{
			StringBuilder info = new StringBuilder();

			info.AppendLine(indent + "Child " + (parentIndex + 1));
			info.AppendLine(indent + "  " + nameof(GlobalName) + "   = " + GlobalName);
			info.AppendLine(indent + "  " + nameof(LocalName) + "    = " + LocalName);
			info.AppendLine(indent + "  " + nameof(AutoDestroy) + "  = " + AutoDestroy);
			info.AppendLine(indent + "  " + nameof(Sleeping) + "     = " + Sleeping);
			info.AppendLine(indent + "  " + nameof(FreeSleeping) + " = " + FreeSleeping);

			info.AppendLine(indent + "  Components (" + components.Count + ")");
			if(addComponents)
			{
				int index = 0;
				foreach(var type in components.Keys)
					info.Append(components[type].ToInfoString(addEntities, ++index, indent + "    "));
			}

			info.AppendLine(indent + "  Systems    (" + systems.Count + ")");
			if(addSystems)
			{
				foreach(var type in systems)
					info.AppendLine(indent + "    " + type.FullName);
			}

			info.AppendLine(indent + "  Children   (" + children.Count + ")");
			if(depth != 0)
			{
				foreach(var child in children)
					info.Append(child.ToInfoString(depth - 1, addComponents, addSystems, addEntities, indent + "    "));
			}
			return info.ToString();
		}
	}
}