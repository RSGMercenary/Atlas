using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Extensions;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Serialize;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atlas.ECS.Entities;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public sealed class AtlasEntity : Hierarchy<IEntity>, IEntity
{
	#region Names
	public static readonly string RootName = "Root";
	public static string UniqueName => Guid.NewGuid().ToString("N");
	#endregion

	public static Func<AtlasEntity> Construct { get; set; }
	public static Action<AtlasEntity> Disposed { get; set; }

	#region Pool
	public static AtlasEntity Get() => Construct?.Invoke() ?? new AtlasEntity();

	public static AtlasEntity Get(string localName) => Get(null, localName);

	public static AtlasEntity Get(string globalName = null, string localName = null)
	{
		var entity = Get();
		entity.GlobalName = globalName;
		entity.LocalName = localName;
		return entity;
	}

	public static AtlasEntity Get(bool isRoot)
	{
		var entity = Get();
		entity.IsRoot = isRoot;
		return entity;
	}
	#endregion

	#region Fields
	private readonly EngineItem<IEntity> EngineItem;
	private string globalName = UniqueName;
	private string localName = UniqueName;
	private int selfSleeping = 0;
	private readonly Sleep<IEntity> Sleep;
	private readonly AutoDispose<IEntity> AutoDispose;
	private readonly Dictionary<Type, IComponent> components = new();
	#endregion

	#region Construct / Dispose
	public AtlasEntity()
	{
		EngineItem = new(this);
		Sleep = new(this);
		AutoDispose = new(this, () => Parent == null);
	}

	public AtlasEntity(bool isRoot) : this() => IsRoot = isRoot;

	public AtlasEntity(string globalName = null, string localName = null) : this()
	{
		GlobalName = globalName;
		LocalName = localName;
	}

	public AtlasEntity(string localName) : this(null, localName) { }

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
		SelfSleeping = 0;
		RemoveListeners();

		Disposed?.Invoke(this);
		//base.Disposing();
		//Since we're cleaning up our base Hierarchy here, I don't think we need to call disposing?
	}
	#endregion

	#region Names
	[JsonProperty(Order = int.MinValue + 2)]
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

	[JsonProperty(Order = int.MinValue + 3)]
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
		if(next == RootName && !IsRoot)
			throw new ArgumentException($"Can't set the name of {nameof(IEntity)} '{current}' to '{RootName}'.");
		if(next != RootName && IsRoot)
			throw new ArgumentException($"Can't set the name of {nameof(IEntity)} '{RootName}' to '{next}'.");

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
		where TKey : class, IComponent => HasComponent(typeof(TKey));

	public bool HasComponent(Type type) => components.ContainsKey(type);
	#endregion

	#region Get
	public TValue GetComponent<TKey, TValue>()
		where TKey : class, IComponent
		where TValue : class, TKey => (TValue)GetComponent(typeof(TKey));

	public TKeyValue GetComponent<TKeyValue>()
		where TKeyValue : class, IComponent => (TKeyValue)GetComponent(typeof(TKeyValue));

	public TValue GetComponent<TValue>(Type type)
		where TValue : class, IComponent => (TValue)GetComponent(type);

	public IComponent GetComponent(Type type) => components.ContainsKey(type) ? components[type] : null;

	public Type GetComponentType(IComponent component) => components.Keys.FirstOrDefault(type => components[type] == component);

	public IReadOnlyDictionary<Type, IComponent> Components => components;

	[JsonProperty(PropertyName = nameof(Components), ObjectCreationHandling = ObjectCreationHandling.Replace, Order = int.MaxValue - 1)]
	private IDictionary<Type, IComponent> JsonPropertyComponents
	{
		get => components;
		set
		{
			foreach(var pair in value)
				AddComponent(pair.Value, pair.Key);
		}
	}

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
		where TKey : class, IComponent
		where TValue : class, TKey, new() => AddComponent<TValue>(typeof(TKey));

	public TValue AddComponent<TKey, TValue>(TValue component)
		where TKey : class, IComponent
		where TValue : class, TKey => AddComponent(component, typeof(TKey));

	public TValue AddComponent<TKey, TValue>(TValue component, int index)
		where TKey : class, IComponent
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

	public IComponent AddComponentAsInterface(IComponent component) => AddComponentAsInterface(component, component.Managers.Count);

	public IComponent AddComponentAsInterface(IComponent component, int index) => AddComponent(component, component.GetInterfaceType(), index);
	#endregion
	#endregion

	#region Remove
	public TValue RemoveComponent<TKey, TValue>()
		where TKey : class, IComponent
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
		components.Keys.ToList().ForEach(type => RemoveComponent(type));
		return true;
	}
	#endregion
	#endregion

	#region Hierarchy
	#region Add
	public IEntity AddChild(string globalName, string localName) => AddChild(Get(globalName, localName));

	public IEntity AddChild(string globalName, string localName, int index) => AddChild(Get(globalName, localName), index);

	public IEntity AddChild(string localName) => AddChild(Get(localName));

	public IEntity AddChild(string localName, int index) => AddChild(Get(localName), index);
	#endregion

	#region Remove
	public IEntity RemoveChild(string localName) => RemoveChild(GetChild(localName));
	#endregion

	#region Get
	public IEntity GetChild(string localName) => Children.FirstOrDefault(child => child.LocalName == localName);

	public IEntity GetRelative(string hierarchy)
	{
		IEntity entity = this;
		foreach(var localName in hierarchy.Split('/'))
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

	public int SelfSleeping
	{
		get => selfSleeping;
		private set
		{
			if(selfSleeping == value)
				return;
			int previous = selfSleeping;
			selfSleeping = value;
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

	public bool IsSelfSleeping
	{
		get => selfSleeping > 0;
		set
		{
			if(value)
				++SelfSleeping;
			else
				--SelfSleeping;
		}
	}
	#endregion

	#region AutoDispose
	[JsonProperty(Order = int.MinValue + 1)]
	public bool IsAutoDisposable
	{
		get => AutoDispose.IsAutoDisposable;
		set => AutoDispose.IsAutoDisposable = value;
	}
	#endregion

	public string Serialize(Formatting formatting = Formatting.None) => AtlasSerializer.Serialize(this, formatting);

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
				if(!IsSelfSleeping)
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
				if(!IsSelfSleeping)
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