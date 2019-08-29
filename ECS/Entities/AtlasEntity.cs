using Atlas.Core.Collections.Group;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Signals;
using Atlas.ECS.Components;
using Atlas.ECS.Entities.Messages;
using Atlas.ECS.Objects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Entities
{
	public sealed class AtlasEntity : AtlasObject<IEntity>, IEntity
	{
		#region Static

		public const string RootName = "Root";
		public static string UniqueName { get { return $"Entity { Guid.NewGuid().ToString("N")}"; } }

		private static readonly Pool<AtlasEntity> pool = new Pool<AtlasEntity>();

		public static IReadOnlyPool<AtlasEntity> Pool() { return pool; }

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
		private string globalName;
		private string localName;
		private int sleeping = 0;
		private int freeSleeping = 0;
		private bool autoDispose = true;
		private HierarchyMessenger<IEntity> hierarchy;
		private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();
		#endregion

		#region Construct / Finalize

		public AtlasEntity() : this("", "", false) { }
		public AtlasEntity(string name) : this(name, name) { }
		public AtlasEntity(string name, bool local) : this(local ? "" : name, local ? name : "") { }
		public AtlasEntity(bool root) : this("", "", root) { }
		public AtlasEntity(string globalName, string localName) : this(globalName, localName, false) { }

		private AtlasEntity(string globalName, string localName, bool root)
		{
			hierarchy = new HierarchyMessenger<IEntity>(this, root);
			if(root)
			{
				this.globalName = RootName;
				this.localName = RootName;
			}
			else
			{
				GlobalName = globalName;
				LocalName = localName;
			}
		}

		protected override void Disposing()
		{
			hierarchy.Dispose();
			RemoveComponents();
			GlobalName = UniqueName;
			LocalName = UniqueName;
			AutoDispose = true;
			Sleeping = 0;
			FreeSleeping = 0;

			pool.Add(this);
			base.Disposing();
		}

		private void TryAutoDispose()
		{
			if(autoDispose && Parent == null)
				Dispose();
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
				if(this == Root || value == RootName)
					return;
				if(string.IsNullOrWhiteSpace(value))
					value = UniqueName;
				else
				{
					if(globalName == value)
						return;
					if(Engine?.HasEntity(value) ?? false)
						return;
				}
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
				//Prevents the Root from changing names and other
				//Entities from chaning their names to Root.
				if(this == Root || value == RootName)
					return;
				if(string.IsNullOrWhiteSpace(value))
					value = UniqueName;
				else
				{
					if(localName == value)
						return;
					if(Parent?.HasChild(value) ?? false)
						return;
				}
				string previous = localName;
				localName = value;
				Message<ILocalNameMessage>(new LocalNameMessage(value, previous));
			}
		}

		#endregion

		#region Engine

		public sealed override IEngine Engine
		{
			get { return base.Engine; }
			set
			{
				if(value != null && Engine == null && value.HasEntity(this))
					base.Engine = value;
				else if(value == null && Engine != null && !Engine.HasEntity(this))
					base.Engine = value;
			}
		}

		#endregion

		#region Components

		#region Has

		public bool HasComponent<TKey>()
			where TKey : IComponent
		{
			return HasComponent(typeof(TKey));
		}

		public bool HasComponent(Type type)
		{
			return components.ContainsKey(type);
		}

		#endregion

		#region Get

		public TValue GetComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey
		{
			return (TValue)GetComponent(typeof(TKey));
		}

		public TKeyValue GetComponent<TKeyValue>()
			where TKeyValue : IComponent
		{
			return (TKeyValue)GetComponent(typeof(TKeyValue));
		}

		public TValue GetComponent<TValue>(Type type)
			where TValue : IComponent
		{
			return (TValue)GetComponent(type);
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

		public TKeyValue GetAncestorComponent<TKeyValue>()
			where TKeyValue : IComponent
		{
			var ancestor = Parent;
			while(ancestor != null)
			{
				var component = ancestor.GetComponent<TKeyValue>();
				if(component != null)
					return component;
				ancestor = ancestor.Parent;
			}
			return default;
		}

		public IEnumerable<TKeyValue> GetDescendantComponents<TKeyValue>()
			where TKeyValue : IComponent
		{
			var components = new List<TKeyValue>();
			foreach(var child in Children)
			{
				var component = child.GetComponent<TKeyValue>();
				if(component != null)
					components.Add(component);
				else
					components.AddRange(child.GetDescendantComponents<TKeyValue>());
			}
			return components;
		}

		#endregion

		#region Add

		//New component with Type
		public TValue AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey, new()
		{
			return (TValue)AddComponent(new TValue(), typeof(TKey), 0);
		}

		//Component with Type
		public TValue AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : TKey
		{
			return (TValue)AddComponent(component, typeof(TKey), int.MaxValue);
		}

		//Component with Type, index
		public TValue AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : TKey
		{
			return (TValue)AddComponent(component, typeof(TKey), index);
		}

		//New Component
		public TKeyValue AddComponent<TKeyValue>()
			where TKeyValue : IComponent, new()
		{
			return (TKeyValue)AddComponent(new TKeyValue(), null, 0);
		}

		//Component
		public TKeyValue AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : IComponent
		{
			return (TKeyValue)AddComponent(component, typeof(TKeyValue), int.MaxValue);
		}

		//Component, index
		public TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : IComponent
		{
			return (TKeyValue)AddComponent(component, typeof(TKeyValue), index);
		}

		public TValue AddComponent<TValue>(TValue component, Type type)
			where TValue : IComponent
		{
			return AddComponent(component, type);
		}

		public TValue AddComponent<TValue>(Type type)
			where TValue : IComponent, new()
		{
			return AddComponent(new TValue(), type);
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
			Message<IComponentAddMessage>(new ComponentAddMessage(type, component));
			return component;
		}

		#endregion

		#region Remove

		public TValue RemoveComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey
		{
			return (TValue)RemoveComponent(typeof(TKey));
		}

		public TKeyValue RemoveComponent<TKeyValue>()
			where TKeyValue : IComponent
		{
			return (TKeyValue)RemoveComponent(typeof(TKeyValue));
		}

		public IComponent RemoveComponent(Type type)
		{
			if(type == null || !components.ContainsKey(type))
				return null;
			var component = components[type];
			components.Remove(type);
			component.RemoveManager(this, type);
			Message<IComponentRemoveMessage>(new ComponentRemoveMessage(type, component));
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

		#endregion

		#region Hierarchy

		#region Root

		public IEntity Root
		{
			get { return hierarchy.Root; }
		}

		#endregion

		#region Parent

		public IEntity Parent
		{
			get { return hierarchy.Parent; }
			set { hierarchy.Parent = value; }
		}

		public IEntity SetParent(IEntity parent = null, int index = int.MaxValue)
		{
			return hierarchy.SetParent(parent, index);
		}

		public int ParentIndex
		{
			get { return hierarchy.ParentIndex; }
			set { hierarchy.ParentIndex = value; }
		}

		#endregion

		#region Add

		#region Pool

		public IEntity AddChild(string globalName, string localName)
		{
			return AddChild(Get(globalName, localName), Children.Count);
		}

		public IEntity AddChild(string globalName, string localName, int index)
		{
			return AddChild(Get(globalName, localName), index);
		}

		public IEntity AddChild(string globalName, bool localName)
		{
			return AddChild(Get(globalName, localName), Children.Count);
		}

		public IEntity AddChild(string globalName, bool localName, int index)
		{
			return AddChild(Get(globalName, localName), index);
		}

		#endregion

		public IEntity AddChild(IEntity child)
		{
			return AddChild(child, Children.Count);
		}

		public IEntity AddChild(IEntity child, int index)
		{
			return hierarchy.AddChild(child, index);
		}

		#endregion

		#region Remove

		public IEntity RemoveChild(IEntity child)
		{
			return hierarchy.RemoveChild(child);
		}

		public IEntity RemoveChild(int index)
		{
			return hierarchy.RemoveChild(index);
		}

		public IEntity RemoveChild(string localName)
		{
			return RemoveChild(GetChild(localName));
		}

		public bool RemoveChildren()
		{
			return hierarchy.RemoveChildren();
		}

		#endregion

		#region Get

		public IReadOnlyGroup<IEntity> Children
		{
			get { return hierarchy.Children; }
		}

		public IEnumerator<IEntity> GetEnumerator()
		{
			return hierarchy.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEntity GetChild(string localName)
		{
			foreach(var child in Children)
			{
				if(child.LocalName == localName)
					return child;
			}
			return null;
		}

		public IEntity GetChild(int index)
		{
			return hierarchy.GetChild(index);
		}

		public int GetChildIndex(IEntity child)
		{
			return hierarchy.GetChildIndex(child);
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
			return hierarchy.SetChildIndex(child, index);
		}

		public bool SwapChildren(IEntity child1, IEntity child2)
		{
			return hierarchy.SwapChildren(child1, child2);
		}

		public bool SwapChildren(int index1, int index2)
		{
			return hierarchy.SwapChildren(index1, index2);
		}

		public IEntity SetHierarchy(string hierarchy, int index)
		{
			return SetParent(GetHierarchy(hierarchy), index);
		}

		#endregion

		#region Has

		public bool HasChild(string localName)
		{
			return GetChild(localName) != null;
		}

		public bool HasChild(IEntity child)
		{
			return hierarchy.HasChild(child);
		}

		public bool HasDescendant(IEntity descendant)
		{
			return hierarchy.HasDescendant(descendant);
		}

		public bool HasAncestor(IEntity ancestor)
		{
			return hierarchy.HasAncestor(ancestor);
		}

		public bool HasSibling(IEntity sibling)
		{
			return hierarchy.HasSibling(sibling);
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
				Message<ISleepMessage<IEntity>>(new SleepMessage<IEntity>(value, previous));
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
				Message<IFreeSleepMessage>(new FreeSleepMessage(value, previous));
				if(Parent == null)
					return;
				if(value > 0 && previous <= 0)
				{
					if(Parent.IsSleeping)
						--Sleeping;
				}
				else if(value <= 0 && previous > 0)
				{
					if(Parent.IsSleeping)
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
				Message<IAutoDisposeMessage<IEntity>>(new AutoDisposeMessage<IEntity>(value, previous));
				TryAutoDispose();
			}
		}

		#endregion

		#region Messages

		protected override Signal<TMessage> CreateSignal<TMessage>()
		{
			return new HierarchySignal<TMessage, IEntity>();
		}

		public void AddListener<TMessage>(Action<TMessage> listener, MessageFlow messenger)
			where TMessage : IMessage<IEntity>
		{
			AddListener(listener, 0, messenger);
		}

		public void AddListener<TMessage>(Action<TMessage> listener, int priority, MessageFlow messenger)
			where TMessage : IMessage<IEntity>
		{
			(AddListenerSlot(listener, priority) as HierarchySlot<TMessage, IEntity>).Messenger = messenger;
		}

		public sealed override void Message<TMessage>(TMessage message)
		{
			Message(message, MessageFlow.All);
		}

		public void Message<TMessage>(TMessage message, MessageFlow flow)
			where TMessage : IMessage<IEntity>
		{
			hierarchy.Message(message, flow, base.Message);
		}

		protected override void Messaging(IMessage<IEntity> message)
		{
			hierarchy.Messaging(message);

			if(message.Messenger == message.CurrentMessenger)
			{
				if(message is IChildAddMessage<IEntity> childAddMessage)
				{
					bool nameTaken = false;
					foreach(var child in Children)
					{
						if(child.LocalName != childAddMessage.Value.LocalName)
							continue;
						if(!nameTaken)
							nameTaken = true;
						else
						{
							childAddMessage.Value.LocalName = UniqueName;
							break;
						}
					}
				}
				else if(message is IParentMessage<IEntity> parentMessage)
				{
					if(!IsFreeSleeping)
					{
						int sleeping = 0;
						if(parentMessage.PreviousValue?.IsSleeping ?? false)
							--sleeping;
						if(parentMessage.CurrentValue?.IsSleeping ?? false)
							++sleeping;
						Sleeping += sleeping;
					}
					TryAutoDispose();
				}
			}
			else if(message.Messenger == Parent)
			{
				if(message is ISleepMessage<IEntity> sleepMessage)
				{
					if(!IsFreeSleeping)
					{
						if(sleepMessage.CurrentValue > 0 && sleepMessage.PreviousValue <= 0)
							++Sleeping;
						else if(sleepMessage.CurrentValue <= 0 && sleepMessage.PreviousValue > 0)
							--Sleeping;
					}
				}
			}
			base.Messaging(message);
		}

		#endregion

		#region Info Strings

		public string AncestorsToString(int depth = -1, bool localNames = true, string indent = "")
		{
			var text = new StringBuilder();
			if(Parent != null && depth != 0)
			{
				var parent = Parent;
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
				foreach(var child in Children)
					text.Append(child.DescendantsToString(--depth, localNames, indent + "  "));
			}
			return text.ToString();
		}

		public override string ToString()
		{
			return GlobalName;
		}

		public string ToInfoString(int depth = -1, bool addComponents = true, bool addEntities = false, string indent = "", StringBuilder text = null)
		{
			text = text ?? new StringBuilder();

			var name = (this == Root) ? RootName : $"Child {ParentIndex + 1}";
			text.AppendLine($"{indent}{name}");
			text.AppendLine($"{indent}  {nameof(GlobalName)}   = {GlobalName}");
			text.AppendLine($"{indent}  {nameof(LocalName)}    = {LocalName}");
			text.AppendLine($"{indent}  {nameof(AutoDispose)}  = {AutoDispose}");
			text.AppendLine($"{indent}  {nameof(Sleeping)}     = {Sleeping}");
			text.AppendLine($"{indent}  {nameof(FreeSleeping)} = {FreeSleeping}");

			text.AppendLine($"{indent}  {nameof(Components)} ({components.Count})");
			if(addComponents)
			{
				int index = 0;
				foreach(var type in components.Keys)
					components[type].ToInfoString(addEntities, ++index, $"{indent}    ", text);
			}

			text.AppendLine($"{indent}  {nameof(Children)}   ({Children.Count})");
			if(depth != 0)
			{
				foreach(var child in Children)
					child.ToInfoString(--depth, addComponents, addEntities, $"{indent}    ", text);
			}
			return text.ToString();
		}

		#endregion
	}
}