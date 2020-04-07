using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Entities
{
	public sealed class AtlasEntity : Hierarchy<IEntity>, IEntity
	{
		#region Static
		public static readonly string RootName = "Root";
		public static string UniqueName => $"Entity {Guid.NewGuid().ToString("N")}";

		#region Pool
		private static readonly Pool<AtlasEntity> pool = new Pool<AtlasEntity>(() => new AtlasEntity());

		public static IReadOnlyPool<AtlasEntity> GetPool() => pool;

		public static AtlasEntity Get() => pool.Get();

		public static AtlasEntity Get(string globalName) => Get(globalName, globalName);

		public static AtlasEntity Get(string globalName, string localName)
		{
			var entity = Get();
			entity.GlobalName = globalName;
			entity.LocalName = localName;
			return entity;
		}

		public static AtlasEntity GetRoot()
		{
			var entity = Get();
			entity.IsRoot = true;
			return entity;
		}

		private static void Release(AtlasEntity entity) => pool.Release(entity);
		#endregion
		#endregion

		#region Fields
		private readonly EngineItem<IEntity> EngineItem;
		private string globalName = UniqueName;
		private string localName = UniqueName;
		private int freeSleeping = 0;
		private readonly Sleep<IEntity> Sleep;
		private readonly AutoDispose<IEntity> AutoDispose;
		private readonly Dictionary<Type, IComponent> components = new Dictionary<Type, IComponent>();

		#endregion

		#region Construct / Dispose
		private AtlasEntity()
		{
			EngineItem = new EngineItem<IEntity>(this, (engine, entity) => engine.HasEntity(entity));
			Sleep = new Sleep<IEntity>(this);
			AutoDispose = new AutoDispose<IEntity>(this, () => Parent == null);
		}

		protected override void Disposing()
		{
			RemoveChildren();
			Parent = null;
			IsRoot = false;
			RemoveComponents();
			GlobalName = UniqueName;
			LocalName = UniqueName;
			IsAutoDisposable = true;
			Sleeping = 0;
			FreeSleeping = 0;
			RemoveListeners();

			AtlasEntity.Release(this);
			//base.Disposing();
			//Since we're cleaning up our base Hierarchy here, I don't think we need to call disposing?
		}
		#endregion

		#region Names
		public string GlobalName
		{
			get => globalName;
			set
			{
				if(!IsValidName(globalName, ref value, n => Engine?.HasEntity(n) ?? false))
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
				if(!IsValidName(localName, ref value, n => Parent?.HasChild(n) ?? false))
					return;
				string previous = localName;
				localName = value;
				Message<ILocalNameMessage>(new LocalNameMessage(value, previous));
			}
		}

		private bool IsValidName(string current, ref string next, Func<string, bool> check)
		{
			if((next == RootName && !IsRoot) || (next != RootName && IsRoot))
				throw new ArgumentException($"Can't set the name of an {nameof(IEntity)} to '{RootName}'.");
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

		public override string ToString() => GlobalName;
		#endregion

		#region Engine
		public IEngine Engine
		{
			get => EngineItem.Engine;
			set => EngineItem.Engine = value;
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
			where TValue : class, TKey => (TValue)GetComponent(typeof(TKey));

		public TKeyValue GetComponent<TKeyValue>()
			where TKeyValue : class, IComponent => (TKeyValue)GetComponent(typeof(TKeyValue));

		public TValue GetComponent<TValue>(Type type)
			where TValue : class, IComponent => (TValue)GetComponent(type);

		public IComponent GetComponent(Type type) => components.ContainsKey(type) ? components[type] : null;

		public Type GetComponentType(IComponent component)
		{
			foreach(var type in components.Keys)
			{
				if(components[type] == component)
					return type;
			}
			return null;
		}

		public IReadOnlyDictionary<Type, IComponent> Components => components;

		public TKeyValue GetAncestorComponent<TKeyValue>(int depth = -1, bool self = false)
			where TKeyValue : class, IComponent
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
			where TKeyValue : class, IComponent
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
		#region KeyValue
		public TKeyValue AddComponent<TKeyValue>()
			where TKeyValue : class, IComponent, new() => AddComponent<TKeyValue, TKeyValue>();

		public TKeyValue AddComponent<TKeyValue>(TKeyValue component)
			where TKeyValue : class, IComponent => AddComponent<TKeyValue, TKeyValue>(component);

		public TKeyValue AddComponent<TKeyValue>(TKeyValue component, int index)
			where TKeyValue : class, IComponent => AddComponent<TKeyValue, TKeyValue>(component, index);
		#endregion

		#region Key, Value
		public TValue AddComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey, new() => AddComponent<TValue>(typeof(TKey));

		public TValue AddComponent<TKey, TValue>(TValue component)
			where TKey : IComponent
			where TValue : class, TKey => AddComponent(component, typeof(TKey));

		public TValue AddComponent<TKey, TValue>(TValue component, int index)
			where TKey : IComponent
			where TValue : class, TKey => AddComponent(component, typeof(TKey), index);
		#endregion

		#region Type, Value
		public TValue AddComponent<TValue>(Type type)
			where TValue : class, IComponent, new() => AddComponent(AtlasComponent.Get<TValue>(), type);

		public TValue AddComponent<TValue>(TValue component, Type type)
			where TValue : class, IComponent => AddComponent(component, type, component.Managers.Count);

		public TValue AddComponent<TValue>(TValue component, Type type, int index)
			where TValue : class, IComponent => (TValue)AddComponent((IComponent)component, type, index);
		#endregion

		#region Type, IComponent
		public IComponent AddComponent(IComponent component) => AddComponent(component, component.Managers.Count);

		public IComponent AddComponent(IComponent component, Type type) => AddComponent(component, type, component.Managers.Count);

		public IComponent AddComponent(IComponent component, int index) => AddComponent(component, null, index);

		public IComponent AddComponent(IComponent component, Type type, int index)
		{
			if(component.HasManager(this))
				return component;
			else if(component.Manager != null)
				throw new InvalidOperationException($"The component '{component.GetType().Name}' is non-shareable and can't be added to another {nameof(IEntity)}.");

			type = AtlasComponent.GetType(component, type);
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
		#endregion

		#region Remove
		public TValue RemoveComponent<TKey, TValue>()
			where TKey : IComponent
			where TValue : class, TKey => (TValue)RemoveComponent(typeof(TKey));

		public TKeyValue RemoveComponent<TKeyValue>()
			where TKeyValue : class, IComponent => (TKeyValue)RemoveComponent(typeof(TKeyValue));

		public IComponent RemoveComponent(Type type)
		{
			if(!components.ContainsKey(type))
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

		public IEntity AddChild(string globalName) => AddChild(Get(globalName), Children.Count);

		public IEntity AddChild(string globalName, int index) => AddChild(Get(globalName), index);
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
			get => Sleep.Sleeping;
			private set => Sleep.Sleeping = value;
		}

		public bool IsSleeping
		{
			get => Sleep.IsSleeping;
			set => Sleep.IsSleeping = value;
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
		public bool IsAutoDisposable
		{
			get => AutoDispose.IsAutoDisposable;
			set => AutoDispose.IsAutoDisposable = value;
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
						int deltaSleeping = 0;
						if(parentMessage.PreviousValue?.IsSleeping ?? false)
							--deltaSleeping;
						if(parentMessage.CurrentValue?.IsSleeping ?? false)
							++deltaSleeping;
						Sleeping += deltaSleeping;
					}
					AutoDispose.TryAutoDispose();
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
	}
}