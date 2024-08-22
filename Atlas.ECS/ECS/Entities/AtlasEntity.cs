using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Extensions;
using Atlas.Core.Messages;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Atlas.ECS.Entities;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public sealed class AtlasEntity : Hierarchy<IEntity>, IEntity
{
	#region Static
	public static readonly string RootName = "Root";
	public static string UniqueName => Guid.NewGuid().ToString("N");

	private static bool IsValidName(string current, string next, bool isRoot, Func<string, bool> check = null)
	{
		if(next == RootName && !isRoot)
			ThrowNameException(current, RootName);
		if(next != RootName && isRoot)
			ThrowNameException(RootName, next);

		if(string.IsNullOrWhiteSpace(next))
			return false;
		else
		{
			if(current == next)
				return false;
			if(check.Invoke(next))
				return false;
		}
		return true;
	}

	private static void ThrowNameException(string current, string next) => throw new ArgumentException($"Can't set the name of {nameof(IEntity)} '{current}' to '{next}'.");
	#endregion

	#region Pool
	public static AtlasEntity Get() => PoolManager.Instance.GetOrNew<AtlasEntity>();

	public static AtlasEntity Get(string localName = null, string globalName = null) { var entity = Get(); entity.SetNames(localName, globalName); return entity; }

	public static AtlasEntity GetRoot() { var root = Get(); root.IsRoot = true; return root; }

	#endregion

	#region Fields
	private string globalName = UniqueName;
	private string localName = UniqueName;
	private int selfSleeping = 0;
	private readonly Sleep<IEntity> Sleep;
	private readonly EngineItem<IEntity> EngineItem;
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

	public AtlasEntity(string localName = null, string globalName = null) : this() { SetNames(localName, globalName); }

	protected override void Disposing()
	{
		RemoveComponents();
		GlobalName = UniqueName;
		LocalName = UniqueName;
		IsAutoDisposable = true;
		Sleeping = 0;
		SelfSleeping = 0;

		base.Disposing();

		PoolManager.Instance.Put(this);
	}
	#endregion

	#region Names
	[JsonProperty(Order = int.MinValue + 2)]
	public string GlobalName
	{
		get => globalName;
		set
		{
			if(!IsValidName(globalName, value, IsRoot, n => Engine?.HasEntity(n) ?? false))
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
			if(!IsValidName(localName, value, IsRoot, n => Parent?.HasChild(n) ?? false))
				return;
			string previous = localName;
			localName = value;
			Message<ILocalNameMessage>(new LocalNameMessage(value, previous));
		}
	}

	public void SetNames(string name) => SetNames(name, name);

	public void SetNames(string localName, string globalName) { LocalName = localName; GlobalName = globalName; }

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
	public TComponent GetComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType => (TComponent)GetComponent(typeof(TType));

	public TType GetComponent<TType>()
		where TType : class, IComponent => (TType)GetComponent(typeof(TType));

	public TComponent GetComponent<TComponent>(Type type)
		where TComponent : class, IComponent => (TComponent)GetComponent(type);

	public IComponent GetComponent(Type type) => components.TryGetValue(type, out var component) ? component : null;

	public Type GetComponentType(IComponent component) => components.Keys.FirstOrDefault(type => components[type] == component);

	[JsonIgnore]
	public IReadOnlyDictionary<Type, IComponent> Components => components;

	[JsonProperty(PropertyName = nameof(Components), Order = int.MaxValue - 1)]
	[ExcludeFromCodeCoverage]
	private IDictionary<Type, IComponent> SerializeComponents
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
	#region Component & Type
	public TComponent AddComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType, new() => AddComponent<TComponent>(typeof(TType));

	public TComponent AddComponent<TComponent, TType>(TComponent component, int index)
		where TType : class, IComponent
		where TComponent : class, TType => (TComponent)AddComponent<TType>(component, index);

	public TComponent AddComponent<TComponent, TType>(TComponent component)
		where TType : class, IComponent
		where TComponent : class, TType => (TComponent)AddComponent<TType>(component);
	#endregion

	#region Component
	public TComponent AddComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent, new() => AddComponent(PoolManager.Instance.GetOrNew<TComponent>(), type);

	public TComponent AddComponent<TComponent>(TComponent component, int index)
		where TComponent : class, IComponent => AddComponent(component, null, index);

	public TComponent AddComponent<TComponent>(TComponent component, Type type = null, int? index = null)
		where TComponent : class, IComponent
	{
		bool isAutoDisposable = false;

		if(component.HasManager(this))
			return component;
		else if(component.Manager != null)
		{
			if(component.IsAutoDisposable)
			{
				isAutoDisposable = true;
				component.IsAutoDisposable = false;
			}
			component.RemoveManagers();
		}

		type = AtlasComponent.GetType(component, type ?? typeof(TComponent));
		if(components.TryGetValue(type, out var current))
		{
			if(component == current)
				return component;
			RemoveComponent<IComponent>(type);
		}
		components.Add(type, component);
		component.AddManager(this, type, index);
		if(isAutoDisposable)
			component.IsAutoDisposable = true;
		Message<IComponentAddMessage>(new ComponentAddMessage(type, component));
		return component;
	}

	public IComponent AddComponentType(IComponent component, int? index = null) => AddComponent(component, component.GetInterfaceType(), index);
	#endregion
	#endregion

	#region Remove
	public TComponent RemoveComponent<TComponent, TType>()
		where TType : class, IComponent
		where TComponent : class, TType => (TComponent)RemoveComponent<TType>();

	public TComponent RemoveComponent<TComponent>(TComponent component, Type type = null)
		where TComponent : class, IComponent => RemoveComponent<TComponent>(type);

	public TComponent RemoveComponent<TComponent>(Type type = null)
		where TComponent : class, IComponent => (TComponent)RemoveComponent(type ?? typeof(TComponent));

	public IComponent RemoveComponent(Type type)
	{
		if(!components.TryGetValue(type, out var component))
			return null;
		components.Remove(type);
		component.RemoveManager(this, type);
		Message<IComponentRemoveMessage>(new ComponentRemoveMessage(type, component));
		return component;
	}

	public TComponent RemoveComponentType<TComponent>(TComponent component)
		where TComponent : class, IComponent => RemoveComponent<TComponent>(GetComponentType(component));

	public bool RemoveComponents()
	{
		if(components.Count <= 0)
			return false;
		components.Keys.ToList().ForEach(type => RemoveComponent<IComponent>(type));
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