using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Objects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.ECS.Entities
{
	public sealed class AtlasEntity : Hierarchy<IEntity>, IEntity
	{
		#region Static

		public const string RootName = "Root";
		public static string UniqueName => $"Entity {Guid.NewGuid().ToString("N")}";

		private static readonly Pool<AtlasEntity> pool = new Pool<AtlasEntity>();

		public static IReadOnlyPool<AtlasEntity> Pool() => pool;

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

		private IEngine engine;
		private string globalName;
		private string localName;
		private int sleeping = 0;
		private int freeSleeping = 0;
		private bool autoDispose = true;
		private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

		#endregion

		#region Construct / Dispose

		public AtlasEntity() : this("", "", false) { }
		public AtlasEntity(string name) : this(name, name) { }
		public AtlasEntity(bool root) : this("", "", root) { }
		public AtlasEntity(string globalName, string localName) : this(globalName, localName, false) { }

		private AtlasEntity(string globalName, string localName, bool root)
		{
			IsRoot = root;
			GlobalName = globalName;
			LocalName = localName;
		}

		protected override void Disposing()
		{
			RemoveChildren();
			Parent = null;
			IsRoot = false;
			RemoveComponents();
			GlobalName = UniqueName;
			LocalName = UniqueName;
			AutoDispose = true;
			Sleeping = 0;
			FreeSleeping = 0;
			RemoveListeners();

			pool.Add(this);
			//base.Disposing();
		}

		#endregion

		#region Names

		public string GlobalName
		{
			get => globalName;
			set
			{
				if(!IsValidName(globalName, ref value, s => Engine?.HasEntity(s) ?? false))
					return;
				string previous = globalName;
				globalName = value;
				Message<IGlobalNameMessage>(new GlobalNameMessage(value, previous));
			}
		}

		public string LocalName
		{
			get => localName;
			set
			{
				if(!IsValidName(localName, ref value, s => Parent?.HasChild(s) ?? false))
					return;
				string previous = localName;
				localName = value;
				Message<ILocalNameMessage>(new LocalNameMessage(value, previous));
			}
		}

		private bool IsValidName(string current, ref string next, Func<string, bool> check)
		{
			if((next == RootName && !IsRoot) || (next != RootName && IsRoot))
				return false;
			if(string.IsNullOrWhiteSpace(next))
				next = UniqueName;
			else
			{
				if(current == next)
					return false;
				if(check.Invoke(next))
					return false;
			}
			return true;
		}

		#endregion

		#region Engine

		public IEngine Engine
		{
			get => engine;
			set
			{
				if(value != null && Engine == null && value.HasEntity(this))
				{
					var previous = engine;
					engine = value;
					Message<IEngineMessage<IEntity>>(new EngineMessage<IEntity>(value, previous));
				}
				else if(value == null && Engine != null && !Engine.HasEntity(this))
				{
					var previous = engine;
					engine = value;
					Message<IEngineMessage<IEntity>>(new EngineMessage<IEntity>(value, previous));
				}
			}
		}

		#endregion

		#region Components

		#region Has

		public bool HasComponent<TKey>()
			where TKey : IComponent => HasComponent(typeof(TKey));

		public bool HasComponent(Type type) => components.ContainsKey(type);

		#endregion

		#region Get

		public TValue GetComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey => (TValue)GetComponent(typeof(TKey));

		public TKeyValue GetComponent<TKeyValue>()
			where TKeyValue : IComponent => (TKeyValue)GetComponent(typeof(TKeyValue));

		public TValue GetComponent<TValue>(Type type)
			where TValue : IComponent => (TValue)GetComponent(type);

		public IComponent GetComponent(Type type) => components.ContainsKey(type) ? components[type] : null;

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

		public IReadOnlyDictionary<Type, IComponent> Components => components;

		public TKeyValue GetAncestorComponent<TKeyValue>(int depth = -1, bool self = false)
			where TKeyValue : IComponent
		{
			var ancestor = self ? this : Parent;
			while(ancestor != null && depth-- != 0)
			{
				var component = ancestor.GetComponent<TKeyValue>();
				if(component != null)
					return component;
				ancestor = ancestor.Parent;
			}
			return default;
		}

		public IEnumerable<TKeyValue> GetDescendantComponents<TKeyValue>(int depth = -1)
			where TKeyValue : IComponent
		{
			foreach(var child in Children)
			{
				var component = child.GetComponent<TKeyValue>();
				if(component != null)
					yield return component;
				else if(depth-- != 0)
				{
					foreach(var comp in child.GetDescendantComponents<TKeyValue>(depth))
						yield return comp;
				}
			}
		}

		#endregion

		#region Add

		public TValue AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : TKey, new() => (TValue)AddComponent(new TValue(), typeof(TKey), 0);

		public TValue AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : TKey => (TValue)AddComponent(component, typeof(TKey), int.MaxValue);

		public TValue AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : TKey => (TValue)AddComponent(component, typeof(TKey), index);

		public TKeyValue AddComponent<TKeyValue>()
			where TKeyValue : IComponent, new() => (TKeyValue)AddComponent(new TKeyValue(), null, 0);

		public TKeyValue AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : IComponent => (TKeyValue)AddComponent(component, typeof(TKeyValue), int.MaxValue);

		public TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : IComponent => (TKeyValue)AddComponent(component, typeof(TKeyValue), index);

		public TValue AddComponent<TValue>(TValue component, Type type)
			where TValue : IComponent => AddComponent(component, type);

		public TValue AddComponent<TValue>(Type type)
			where TValue : IComponent, new() => AddComponent(new TValue(), type);

		public IComponent AddComponent(IComponent component) => AddComponent(component, null, int.MaxValue);

		public IComponent AddComponent(IComponent component, Type type) => AddComponent(component, type, int.MaxValue);

		public IComponent AddComponent(IComponent component, int index) => AddComponent(component, null, index);

		public IComponent AddComponent(IComponent component, Type type, int index)
		{
			type = type ?? component?.GetType();
			if(!(type?.IsInstanceOfType(component) ?? false))
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
			where TValue : TKey => (TValue)RemoveComponent(typeof(TKey));

		public TKeyValue RemoveComponent<TKeyValue>()
			where TKeyValue : IComponent => (TKeyValue)RemoveComponent(typeof(TKeyValue));

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

		public IComponent RemoveComponent(IComponent component) => RemoveComponent(GetComponentType(component));

		public bool RemoveComponents()
		{
			if(components.Count <= 0)
				return false;
			//Can't Remove() from a Dictionary while iterating its Keys.
			foreach(var type in new List<Type>(components.Keys))
				RemoveComponent(type);
			return true;
		}

		#endregion

		#endregion

		#region Hierarchy

		#region Add

		public IEntity AddChild(string globalName, string localName) => AddChild(Get(globalName, localName), Children.Count);

		public IEntity AddChild(string globalName, string localName, int index) => AddChild(Get(globalName, localName), index);

		public IEntity AddChild(string globalName, bool localName) => AddChild(Get(globalName, localName), Children.Count);

		public IEntity AddChild(string globalName, bool localName, int index) => AddChild(Get(globalName, localName), index);

		#endregion

		#region Remove

		public IEntity RemoveChild(string localName) => RemoveChild(GetChild(localName));

		#endregion

		#region Get

		public IEntity GetChild(string localName)
		{
			foreach(var child in Children)
			{
				if(child.LocalName == localName)
					return child;
			}
			return null;
		}

		public IEntity GetRelative(string hierarchy)
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

		public IEntity SetRelative(string hierarchy, int index) => SetParent(GetRelative(hierarchy), index);

		#endregion

		#region Has

		public bool HasChild(string localName) => GetChild(localName) != null;

		#endregion

		#endregion

		#region Sleep

		public int Sleeping
		{
			get => sleeping;
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
			get => sleeping > 0;
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
			get => freeSleeping;
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
			get => freeSleeping > 0;
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
			get => autoDispose;
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

		private void TryAutoDispose()
		{
			if(autoDispose && Parent == null)
				Dispose();
		}

		#endregion

		#region Messages

		protected override void Messaging(IMessage<IEntity> message)
		{
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
				else if(message is IRootMessage<IEntity> rootMessage)
				{
					if(rootMessage.CurrentValue == this)
					{
						GlobalName = RootName;
						LocalName = RootName;
					}
					else if(rootMessage.PreviousValue == this)
					{
						string uniqueName = UniqueName;
						GlobalName = uniqueName;
						LocalName = uniqueName;
					}
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
				while(ancestor != null && depth-- != 0)
				{
					text.Append("  ");
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
			if(depth-- != 0)
			{
				foreach(var child in Children)
					text.Append(child.DescendantsToString(depth, localNames, indent + "  "));
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

			var name = (IsRoot) ? GlobalName : $"Child {ParentIndex + 1}";
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
			if(depth-- != 0)
			{
				foreach(var child in Children)
					child.ToInfoString(depth, addComponents, addEntities, $"{indent}    ", text);
			}
			return text.ToString();
		}

		#endregion
	}
}