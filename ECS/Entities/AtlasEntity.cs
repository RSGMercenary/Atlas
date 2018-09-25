using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Messages;
using Atlas.ECS.Objects;
using Atlas.ECS.Systems;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Entities
{
	public sealed class AtlasEntity : AtlasObject<IEntity>, IEntity
	{
		#region Static Singleton

		private static AtlasEntity instance;
		public const string RootName = "Root";
		public static string UniqueName { get { return $"Entity {Guid.NewGuid().ToString("N")}"; } }

		private static Pool<AtlasEntity> pool = new Pool<AtlasEntity>(() => new AtlasEntity(), entity => entity.Compose());

		public static IReadOnlyPool<AtlasEntity> Pool() { return pool; }

		public static AtlasEntity Get(string globalName = "", string localName = "")
		{
			var entity = pool.Remove();
			entity.GlobalName = globalName;
			entity.localName = localName;
			return entity;
		}

		#endregion

		private string globalName = UniqueName;
		private string localName = UniqueName;
		private int sleeping = 0;
		private int freeSleeping = 0;
		private IEntity root;
		private IEntity parent;
		private int parentIndex = -1;
		private readonly Group<IEntity> children = new Group<IEntity>();
		private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		private readonly Group<Type> systems = new Group<Type>();
		private bool autoDestroy = true;

		public AtlasEntity()
		{

		}

		public AtlasEntity(string globalName, string localName) : this(globalName)
		{
			LocalName = localName;
		}

		public AtlasEntity(string globalName)
		{
			GlobalName = globalName;
		}

		public AtlasEntity(bool root)
		{
			if(root && instance == null)
			{
				globalName = RootName;
				localName = RootName;
				this.root = this;
				instance = this;
			}
		}

		protected override void Disposing(bool finalizer)
		{
			if(this == instance)
				instance = null;

			//Order matters here!
			//  |
			// \ /
			RemoveChildren();
			RemoveComponents();
			RemoveSystems();
			// / \
			//  |

			GlobalName = UniqueName;
			LocalName = UniqueName;
			AutoDestroy = true;
			Parent = null;
			Sleeping = 0;
			FreeSleeping = 0;
			Root = null;

			//if(!finalizer)
			pool.Add(this);

			base.Disposing(finalizer);
		}

		#region Entity

		public string GlobalName
		{
			get { return globalName; }
			set
			{
				//Prevents the Root from changing names and other
				//Entities from chaning their names to Root.
				if(this == instance || value == RootName)
					return;
				if(string.IsNullOrWhiteSpace(value))
					return;
				if(globalName == value)
					return;
				if(Engine != null && Engine.HasEntity(value))
					return;
				string previous = globalName;
				globalName = value;
				Dispatch<IGlobalNameMessage>(new GlobalNameMessage(this, value, previous));
			}
		}

		public string LocalName
		{
			get { return localName; }
			set
			{
				//Prevents the Root from changing names and other
				//Entities from chaning their names to Root.
				if(this == instance || value == RootName)
					return;
				if(string.IsNullOrWhiteSpace(value))
					return;
				if(localName == value)
					return;
				if(parent != null && parent.HasChild(value))
					return;
				string previous = localName;
				localName = value;
				Dispatch<ILocalNameMessage>(new LocalNameMessage(this, value, previous));
			}
		}

		public string AncestorsToString(int depth = -1, bool localNames = true, string indent = "")
		{
			var text = new StringBuilder();
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
			var text = new StringBuilder();
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
					entity = entity.Parent;
				else
					entity = entity.GetChild(localName);
				if(entity == null)
					break;
			}
			return entity;
		}

		public IEntity SetHierarchy(string hierarchy, int index)
		{
			return SetParent(GetHierarchy(hierarchy), index);
		}

		public IEntity GetChild(string localName)
		{
			foreach(var child in children)
			{
				if(child.LocalName == localName)
					return child;
			}
			return null;
		}

		#endregion

		#region Engine

		public sealed override IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(value != null)
				{
					if(Engine == null && value.HasEntity(this))
						base.Engine = value;
				}
				else
				{
					if(Engine != null && !Engine.HasEntity(this))
						base.Engine = value;
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

		public TComponent GetComponent<TComponent>(Type type)
			where TComponent : IComponent
		{
			return (TComponent)GetComponent(type);
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

		public IReadOnlyDictionary<Type, IComponent> Components
		{
			get { return components; }
		}

		//New component with Type
		public TComponent AddComponent<TComponent, TIComponent>()
			where TIComponent : IComponent
			where TComponent : TIComponent, new()
		{
			return (TComponent)AddComponent(new TComponent(), typeof(TIComponent), 0);
		}

		//Component with Type
		public TComponent AddComponent<TComponent, TIComponent>(TComponent component)
			where TIComponent : IComponent
			where TComponent : TIComponent
		{
			return (TComponent)AddComponent(component, typeof(TIComponent), int.MaxValue);
		}

		//Component with Type, index
		public TComponent AddComponent<TComponent, TIComponent>(TComponent component, int index)
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
		public TIComponent AddComponent<TIComponent>(TIComponent component)
			where TIComponent : IComponent
		{
			return (TIComponent)AddComponent(component, typeof(TIComponent), int.MaxValue);
		}

		//Component, index
		public TIComponent AddComponent<TIComponent>(TIComponent component, int index)
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
			type = type ?? component?.GetType();
			if(!(bool)type?.IsInstanceOfType(component))
				return null;
			//The component isn't shareable and it already has a manager.
			//Or this Entity alreay manages this Component.
			if(component.Manager != null)
			{
				if(component.Manager == this)
					return component;
				return null;
			}
			if(components.ContainsKey(type))
			{
				if(components[type] == component)
					return component;
				RemoveComponent(type);
			}
			components.Add(type, component);
			component.AddManager(this, type, index);
			Dispatch<IComponentAddMessage>(new ComponentAddMessage(this, type, component));
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
			if(type == null || !components.ContainsKey(type))
				return null;
			var component = components[type];
			components.Remove(type);
			component.RemoveManager(this, type);
			Dispatch<IComponentRemoveMessage>(new ComponentRemoveMessage(this, type, component));
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
				//Only need this if Root setter becomes public
				/*if(Parent?.Root != value)
					return;*/
				if(root == value)
					return;
				var previous = root;
				root = value;
				Dispatch<IRootMessage>(new RootMessage(this, value, previous));
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

		public IEntity AddChild(string globalName = "", string localName = "")
		{
			return AddChild(Get(globalName, localName), children.Count);
		}

		public IEntity AddChild(int index, string globalName = "", string localName = "")
		{
			return AddChild(Get(globalName, localName), index);
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
			if(child?.Parent == this)
			{
				if(!HasChild(child))
				{
					if(HasChild(child.LocalName))
						child.LocalName = UniqueName;
					children.Insert(index, child);
					Dispatch<IChildAddMessage>(new ChildAddMessage(this, index, child));
					Dispatch<IChildrenMessage>(new ChildrenMessage(this));
				}
				else
				{
					SetChildIndex(child, index);
				}
			}
			else
			{
				if(child?.SetParent(this, index) == null)
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
				children.Remove(child);
				Dispatch<IChildRemoveMessage>(new ChildRemoveMessage(this, index, child));
				Dispatch<IChildrenMessage>(new ChildrenMessage(this));
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
				child.Dispose();
			return true;
		}

		public IEntity Parent
		{
			get { return parent; }
			set { SetParent(value); }
		}

		public IEntity SetParent(IEntity next = null, int index = int.MaxValue)
		{
			//Prevent changing the Parent of the Root Entity.
			//The Root must be the absolute bottom of the hierarchy.
			if(this == instance)
				return null;
			if(parent == next)
				return null;
			//Can't set a descendant of this as a parent.
			if(HasDescendant(next))
				return null;
			Root = next?.Root;
			var previous = parent;
			int sleeping = 0;
			//TO-DO This may need more checking if parent multi-setting happens during Dispatches.
			if(previous != null)
			{
				parent = null;
				previous.RemoveChild(this);
				if(!IsFreeSleeping && previous.IsSleeping)
					--sleeping;
			}
			if(next != null)
			{
				parent = next;
				index = Math.Max(0, Math.Min(index, next.Children.Count));
				next.AddChild(this, index);
				if(!IsFreeSleeping && next.IsSleeping)
					++sleeping;
			}
			Dispatch<IParentMessage>(new ParentMessage(this, next, previous));
			SetParentIndex(index);
			Sleeping += sleeping;
			if(autoDestroy && parent == null)
				Dispose();
			return next;
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set { parent?.SetChildIndex(this, value); }
		}

		public bool HasDescendant(IEntity descendant)
		{
			if(descendant == this)
				return false;
			while(descendant != null && descendant != this)
				descendant = descendant.Parent;
			return descendant == this;
		}

		public bool HasAncestor(IEntity ancestor)
		{
			return ancestor?.HasDescendant(this) ?? false;
		}

		public bool HasSibling(IEntity sibling)
		{
			if(sibling == this)
				return false;
			return Parent?.HasChild(sibling) ?? false;
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
			Dispatch<IChildrenMessage>(new ChildrenMessage(this));
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
			Dispatch<IChildrenMessage>(new ChildrenMessage(this));
			return true;
		}

		public IReadOnlyGroup<IEntity> Children
		{
			get { return children; }
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			Dispatch<IParentIndexMessage>(new ParentIndexMessage(this, value, previous));
		}

		#endregion

		#region Sleep

		public int Sleeping
		{
			get { return sleeping; }
			private set
			{
				if(sleeping == value)
					return;
				int previous = sleeping;
				sleeping = value;
				Dispatch<ISleepMessage<IEntity>>(new SleepMessage<IEntity>(this, value, previous));
			}
		}

		public bool IsSleeping
		{
			get { return sleeping > 0; }
			set
			{
				if(value)
					++Sleeping;
				else
					--Sleeping;
			}
		}

		public int FreeSleeping
		{
			get { return freeSleeping; }
			private set
			{
				if(freeSleeping == value)
					return;
				int previous = freeSleeping;
				freeSleeping = value;
				Dispatch<IFreeSleepMessage>(new FreeSleepMessage(this, value, previous));
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
			set
			{
				if(value)
					++FreeSleeping;
				else
					--FreeSleeping;
			}
		}

		#endregion

		#region Systems

		public IReadOnlyGroup<Type> Systems
		{
			get { return systems; }
		}

		public bool HasSystem<TISystem>() where TISystem : ISystem
		{
			return HasSystem(typeof(TISystem));
		}

		public bool HasSystem(Type type)
		{
			return systems.Contains(type);
		}

		public bool AddSystem<TISystem>() where TISystem : ISystem
		{
			return AddSystem(typeof(TISystem));
		}

		public bool AddSystem(Type type)
		{
			if(type == null)
				return false;
			if(!type.IsInterface) //Type must be an interface.
				return false;
			if(type == typeof(ISystem)) //Type can't directly be ISystem.
				return false;
			if(!typeof(ISystem).IsAssignableFrom(type)) //Type must be a subclass of ISystem.
				return false;
			if(systems.Contains(type))
				return false;
			systems.Add(type);
			Dispatch<ISystemTypeAddMessage>(new SystemTypeAddMessage(this, type));
			return true;
		}

		public bool RemoveSystem<TISystem>() where TISystem : ISystem
		{
			return RemoveSystem(typeof(TISystem));
		}

		public bool RemoveSystem(Type type)
		{
			if(type == null)
				return false;
			if(!systems.Contains(type))
				return false;
			systems.Remove(type);
			Dispatch<ISystemTypeRemoveMessage>(new SystemTypeRemoveMessage(this, type));
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
				Dispatch<IAutoDestroyMessage<IEntity>>(new AutoDestroyMessage<IEntity>(this, value, previous));
				if(autoDestroy && parent == null)
					Dispose();
			}
		}

		#region Messages

		public sealed override void Dispatch<TMessage>(TMessage message)
		{
			//Keep track of what child told the parent to Dispatch().
			var previousTarget = message.CurrentMessenger;

			//Standard Dispatch() call.
			//Set CurrentMessenger and Dispatch() event by type;
			base.Dispatch(message);

			//Send Message to children.
			foreach(var child in children)
			{
				//Don't send Message back to child that told this to Dispatch().
				if(child == previousTarget)
					continue;
				child.Dispatch(message);
				//Reset CurrentTarget back to this so
				//the next child (and parent) can block messaging from its original source.
				message.CurrentMessenger = this;
			}

			//Send Message to parent.
			if(Parent != null)
			{
				//Don't send Message back to parent that told this to Dispatch().
				if(Parent != previousTarget)
					Parent.Dispatch(message);
			}
			else
			{
				//Parent is null. Can't traverse the hierarchy anymore.
				//Pool message for reuse.
				//PoolManager.Push(message);
			}
		}

		protected override void Messaging(IMessage message)
		{
			if(message.Messenger == Parent)
			{
				if(message is ISleepMessage<IEntity>)
				{
					if(!IsFreeSleeping)
					{
						var cast = message as ISleepMessage<IEntity>;
						if(cast.CurrentValue > 0 && cast.PreviousValue <= 0)
							++Sleeping;
						else if(cast.CurrentValue <= 0 && cast.PreviousValue > 0)
							--Sleeping;
					}
				}
				else if(message is IRootMessage)
				{
					Root = (message as IRootMessage).Messenger.Root;
				}
				else if(message is IChildrenMessage)
				{
					SetParentIndex(Parent.GetChildIndex(this));
				}
			}
			base.Messaging(message);
		}

		#endregion

		public override string ToString()
		{
			return GlobalName;
		}

		public string ToInfoString(int depth, bool addComponents, bool addEntities, bool addSystems, string indent = "")
		{
			var info = new StringBuilder();

			var name = (this == instance) ? RootName : $"Child {parentIndex + 1}";
			info.AppendLine($"{indent}{name}");
			info.AppendLine($"{indent}  {nameof(GlobalName)}   = {GlobalName}");
			info.AppendLine($"{indent}  {nameof(LocalName)}    = {LocalName}");
			info.AppendLine($"{indent}  {nameof(AutoDestroy)}  = {AutoDestroy}");
			info.AppendLine($"{indent}  {nameof(Sleeping)}     = {Sleeping}");
			info.AppendLine($"{indent}  {nameof(FreeSleeping)} = {FreeSleeping}");

			info.AppendLine($"{indent}  {nameof(Components)} ({components.Count})");
			if(addComponents)
			{
				int index = 0;
				foreach(var type in components.Keys)
					info.Append(components[type].ToInfoString(addEntities, ++index, $"{indent}    "));
			}
			info.AppendLine($"{indent}  {nameof(Systems)}    ({systems.Count})");
			if(addSystems)
			{
				foreach(var type in systems)
					info.AppendLine($"{indent}    {type.FullName}");
			}

			info.AppendLine($"{indent}  {nameof(Children)}   ({children.Count})");
			if(depth != 0)
			{
				foreach(var child in children)
					info.Append(child.ToInfoString(depth - 1, addComponents, addSystems, addEntities, $"{indent}    "));
			}
			return info.ToString();
		}
	}
}