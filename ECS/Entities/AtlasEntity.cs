using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.ECS.Components;
using Atlas.ECS.Entities.Messages;
using Atlas.ECS.Objects;
using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Entities
{
	public sealed class AtlasEntity : AtlasObject, IEntity
	{
		#region Static

		private static IEntity instance;
		public const string RootName = "Root";
		public static string UniqueName { get { return $"Entity {Guid.NewGuid().ToString("N")}"; } }

		private static Pool<IEntity> pool = new Pool<IEntity>(() => new AtlasEntity());

		public static IReadOnlyPool<IEntity> Pool() { return pool; }

		public static IEntity Get(string globalName, string localName)
		{
			var entity = Get(globalName, false);
			entity.LocalName = localName;
			return entity;
		}

		public static IEntity Get(string globalName, bool localName)
		{
			var entity = pool.Remove();
			entity.GlobalName = globalName;
			if(localName)
				entity.LocalName = globalName;
			return entity;
		}

		#endregion

		#region Fields

		private string globalName = UniqueName;
		private string localName = UniqueName;
		private int sleeping = 0;
		private int freeSleeping = 0;
		private bool autoDispose = true;
		private IEntity root;
		private int rootIndex = 0;
		private IEntity parent;
		private int parentIndex = -1;
		private readonly Group<IEntity> children = new Group<IEntity>();
		private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

		private Matrix globalMatrix;
		private Matrix localMatrix;

		#endregion

		#region Construct / Finalize

		public AtlasEntity()
		{
			LocalMatrix = Matrix.CreateScale(Vector3.One) *
						  Matrix.CreateFromAxisAngle(Vector3.Backward, 0) *
						  Matrix.CreateTranslation(Vector3.Zero);
		}

		public AtlasEntity(string globalName, string localName) : this(globalName, false)
		{
			LocalName = localName;
		}

		public AtlasEntity(string globalName, bool local = false) : this()
		{
			GlobalName = globalName;
			if(local)
				LocalName = globalName;
		}

		public AtlasEntity(bool root) : this()
		{
			if(root)
			{
				if(instance != null)
					throw new InvalidOperationException($"A new root {GetType().Name} instance cannot be instantiated when one already exists.");
				globalName = RootName;
				localName = RootName;
				this.root = this;
				instance = this;
			}
		}

		protected override void Destroying()
		{
			if(this == instance)
				instance = null;
			base.Destroying();
		}

		protected override void Disposing()
		{
			if(this == instance)
				instance = null;
			RemoveChildren();
			RemoveComponents();
			GlobalName = UniqueName;
			LocalName = UniqueName;
			AutoDispose = true;
			Parent = null;
			Sleeping = 0;
			FreeSleeping = 0;
			Root = null;

			pool.Add(this);
			base.Disposing();
		}

		#endregion

		#region Names

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
				Message<IGlobalNameMessage>(new GlobalNameMessage(this, value, previous));
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
				Message<ILocalNameMessage>(new LocalNameMessage(this, value, previous));
			}
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

		public bool HasComponent<TKey>()
			where TKey : IComponent
		{
			return HasComponent(typeof(TKey));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		public TValue GetComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey
		{
			return GetComponent(typeof(TKey)) as TValue;
		}

		public TKeyValue GetComponent<TKeyValue>()
			where TKeyValue : class, IComponent
		{
			return GetComponent(typeof(TKeyValue)) as TKeyValue;
		}

		public TValue GetComponent<TValue>(Type type)
			where TValue : class, IComponent
		{
			return GetComponent(type) as TValue;
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

		#region Add

		//New component with Type
		public TValue AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey, new()
		{
			return AddComponent(new TValue(), typeof(TKey), 0) as TValue;
		}

		//Component with Type
		public TValue AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : class, TKey
		{
			return AddComponent(component, typeof(TKey), int.MaxValue) as TValue;
		}

		//Component with Type, index
		public TValue AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : class, TKey
		{
			return AddComponent(component, typeof(TKey), index) as TValue;
		}

		//New Component
		public TKeyValue AddComponent<TKeyValue>()
			where TKeyValue : class, IComponent, new()
		{
			return AddComponent(new TKeyValue(), null, 0) as TKeyValue;
		}

		//Component
		public TKeyValue AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : class, IComponent
		{
			return AddComponent(component, typeof(TKeyValue), int.MaxValue) as TKeyValue;
		}

		//Component, index
		public TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : class, IComponent
		{
			return AddComponent(component, typeof(TKeyValue), index) as TKeyValue;
		}

		public TValue AddComponent<TValue>(TValue component, Type type)
			where TValue : class, IComponent
		{
			return AddComponent(component, type) as TValue;
		}

		public TValue AddComponent<TValue>(Type type)
			where TValue : class, IComponent, new()
		{
			return AddComponent(new TValue(), type) as TValue;
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
			if(type == null || !type.IsInstanceOfType(component))
				return null;
			//The component isn't shareable and it already has a manager.
			//Or this Entity alreay manages this Component.
			//TO-DO Fix this so it handles all managers.
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
			Message<IComponentAddMessage>(new ComponentAddMessage(this, type, component));
			return component;
		}

		#endregion

		public TValue RemoveComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey
		{
			return RemoveComponent(typeof(TKey)) as TValue;
		}

		public TKeyValue RemoveComponent<TKeyValue>()
			where TKeyValue : class, IComponent
		{
			return RemoveComponent(typeof(TKeyValue)) as TKeyValue;
		}

		public IComponent RemoveComponent(Type type)
		{
			if(type == null || !components.ContainsKey(type))
				return null;
			var component = components[type];
			components.Remove(type);
			component.RemoveManager(this, type);
			Message<IComponentRemoveMessage>(new ComponentRemoveMessage(this, type, component));
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

		#region Root

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
				Message<IRootMessage>(new RootMessage(this, value, previous));
			}
		}

		public int RootIndex
		{
			get { return rootIndex; }
			private set
			{
				if(rootIndex == value)
					return;
				var previous = rootIndex;
				rootIndex = value;
				Message<IRootIndexMessage>(new RootIndexMessage(this, value, previous));
			}
		}

		#endregion

		#region Parent

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
			Message<IParentMessage>(new ParentMessage(this, next, previous));
			SetParentIndex(next != null ? index : -1);
			SetMatrix();
			Sleeping += sleeping;
			if(autoDispose && parent == null)
				Dispose();
			return next;
		}

		public int ParentIndex
		{
			get { return parentIndex; }
			set { parent?.SetChildIndex(this, value); }
		}

		private void SetParentIndex(int value)
		{
			if(parentIndex == value)
				return;
			int previous = parentIndex;
			parentIndex = value;
			Message<IParentIndexMessage>(new ParentIndexMessage(this, value, previous));
		}

		#endregion

		#region Add

		#region Pool

		public IEntity AddChild(string globalName, string localName)
		{
			return AddChild(Get(globalName, localName), children.Count);
		}

		public IEntity AddChild(string globalName, string localName, int index)
		{
			return AddChild(Get(globalName, localName), index);
		}

		public IEntity AddChild(string globalName, bool localName)
		{
			return AddChild(Get(globalName, localName), children.Count);
		}

		public IEntity AddChild(string globalName, bool localName, int index)
		{
			return AddChild(Get(globalName, localName), index);
		}

		#endregion

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
					Message<IChildAddMessage>(new ChildAddMessage(this, index, child));
					Message<IChildrenMessage>(new ChildrenMessage(this));
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

		#endregion

		#region Remove

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
				Message<IChildRemoveMessage>(new ChildRemoveMessage(this, index, child));
				Message<IChildrenMessage>(new ChildrenMessage(this));
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
				child.Parent = null;
			return true;
		}

		#endregion

		#region Get

		public IReadOnlyGroup<IEntity> Children
		{
			get { return children; }
		}

		public IEnumerator<IEntity> GetEnumerator()
		{
			return children.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
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

		public IEntity GetChild(int index)
		{
			return children[index];
		}

		public int GetChildIndex(IEntity child)
		{
			return children.IndexOf(child);
		}

		public IEntity GetHierarchy(string hierarchy)
		{
			if(string.IsNullOrWhiteSpace(hierarchy))
				return null;
			string[] localNames = hierarchy.Split('/');
			IEntity entity = this;
			foreach(var localName in localNames)
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

		#endregion

		#region Set

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
			Message<IChildrenMessage>(new ChildrenMessage(this));
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
			Message<IChildrenMessage>(new ChildrenMessage(this));
			return true;
		}

		public IEntity SetHierarchy(string hierarchy, int index)
		{
			return SetParent(GetHierarchy(hierarchy) ?? parent, index);
		}

		#endregion

		#region Has

		public bool HasChild(string localName)
		{
			return GetChild(localName) != null;
		}

		public bool HasChild(IEntity child)
		{
			return children.Contains(child);
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
			return parent?.HasChild(sibling) ?? false;
		}

		#endregion

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
				Message<ISleepMessage>(new SleepMessage(this, value, previous));
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
				Message<IFreeSleepMessage>(new FreeSleepMessage(this, value, previous));
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

		#region AutoDispose

		public bool AutoDispose
		{
			get { return autoDispose; }
			set
			{
				if(autoDispose == value)
					return;
				var previous = autoDispose;
				autoDispose = value;
				Message<IAutoDisposeMessage>(new AutoDisposeMessage(this, value, previous));
				if(autoDispose && parent == null)
					Dispose();
			}
		}

		#endregion

		#region Matrix

		public Matrix GlobalMatrix
		{
			get { return globalMatrix; }
			private set
			{
				if(globalMatrix == value)
					return;
				var previous = globalMatrix;
				globalMatrix = value;
				Message<IGlobalMatrixMessage>(new GlobalMatrixMessage(this, value, previous));
			}
		}

		public Matrix LocalMatrix
		{
			get { return localMatrix; }
			set
			{
				if(localMatrix == value)
					return;
				var previous = localMatrix;
				localMatrix = value;
				Message<ILocalMatrixMessage>(new LocalMatrixMessage(this, value, previous));
				SetMatrix();
			}
		}

		private void SetMatrix()
		{
			var matrix = localMatrix;
			if(parent != null)
				matrix *= parent.GlobalMatrix;
			GlobalMatrix = matrix;
		}

		#endregion

		#region Messages

		public sealed override void Message<TMessage>(TMessage message)
		{
			//Keep track of what child told the parent to Dispatch().
			var previousMessenger = message.CurrentMessenger;

			//Standard Message() call.
			//Set CurrentMessenger and Message() event by type;
			base.Message(message);

			//Send Message to children.
			foreach(var child in children)
			{
				//Don't send Message back to child that told this to Message().
				if(child == previousMessenger)
					continue;
				child.Message(message);
				//Reset CurrentMessenger back to this so the next child (and parent)
				//can block messaging from its original source.
				message.CurrentMessenger = this;
			}

			//Send Message to parent.
			if(parent != null)
			{
				//Don't send Message back to parent that told this to Message().
				if(parent != previousMessenger)
					parent.Message(message);
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
			if(message.Messenger == parent)
			{
				if(message is ISleepMessage)
				{
					if(!IsFreeSleeping)
					{
						var cast = message as ISleepMessage;
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
					SetParentIndex(parent.GetChildIndex(this));
				}
				else if(message is IGlobalMatrixMessage)
				{
					SetMatrix();
				}
			}
			if(message is IHierarchyMessage)
			{
				RootIndex = (message as IHierarchyMessage).Value;
			}
			if(parent == null)
			{
				if(message is IChildrenMessage)
					Message<IHierarchyMessage>(new HierarchyMessage(this));
				else if(message is IParentMessage && message.Messenger == this)
					Message<IHierarchyMessage>(new HierarchyMessage(this));
			}
			base.Messaging(message);
		}

		#endregion

		#region Info Strings

		public string AncestorsToString(int depth = -1, bool localNames = true, string indent = "")
		{
			var text = new StringBuilder();
			if(parent != null && depth != 0)
			{
				text.Append(parent.AncestorsToString(depth - 1, localNames, indent));
				var ancestor = parent;
				while(ancestor != null && depth != 0)
				{
					text.Append("  ");
					--depth;
					ancestor = ancestor.Parent;
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

		public override string ToString()
		{
			return GlobalName;
		}

		public string ToInfoString(int depth = -1, bool addComponents = true, bool addEntities = false, string indent = "")
		{
			var info = new StringBuilder();

			var name = (this == instance) ? RootName : $"Child {parentIndex + 1}";
			info.AppendLine($"{indent}{name}");
			info.AppendLine($"{indent}  {nameof(GlobalName)}   = {GlobalName}");
			info.AppendLine($"{indent}  {nameof(LocalName)}    = {LocalName}");
			info.AppendLine($"{indent}  {nameof(AutoDispose)}  = {AutoDispose}");
			info.AppendLine($"{indent}  {nameof(Sleeping)}     = {Sleeping}");
			info.AppendLine($"{indent}  {nameof(FreeSleeping)} = {FreeSleeping}");
			info.AppendLine($"{indent}  {nameof(RootIndex)}    = {RootIndex}");

			info.AppendLine($"{indent}  {nameof(Components)} ({components.Count})");
			if(addComponents)
			{
				int index = 0;
				foreach(var type in components.Keys)
					info.Append(components[type].ToInfoString(addEntities, ++index, $"{indent}    "));
			}

			info.AppendLine($"{indent}  {nameof(Children)}   ({children.Count})");
			if(depth != 0)
			{
				foreach(var child in children)
					info.Append(child.ToInfoString(depth - 1, addComponents, addEntities, $"{indent}    "));
			}
			return info.ToString();
		}

		#endregion
	}
}