using Atlas.Core.Collections.Hierarchy;
using Atlas.Core.Collections.LinkList;
using Atlas.Core.Collections.Pool;
using Atlas.Core.Extensions;
using Atlas.Core.Objects.AutoDispose;
using Atlas.Core.Objects.Sleep;
using Atlas.ECS.Components.Component;
using Atlas.ECS.Components.Engine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Atlas.ECS.Entities;

[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
public sealed class AtlasEntity : IEntity
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
	public static AtlasEntity Get() => PoolManager.Instance.Get<AtlasEntity>();

	public static AtlasEntity Get(string localName = null, string globalName = null) { var entity = Get(); entity.SetNames(localName, globalName); return entity; }

	public static AtlasEntity GetRoot() { var root = Get(); root.IsRoot = true; return root; }
	#endregion

	#region Events
	#region Engine
	public event Action<IEntity, IEngine, IEngine> EngineChanged
	{
		add => EngineManager.EngineChanged += value;
		remove => EngineManager.EngineChanged -= value;
	}
	#endregion

	#region Hierarchy
	public event Action<IEntity, IEntity, IEntity> RootChanged
	{
		add => Hierarchy.RootChanged += value;
		remove => Hierarchy.RootChanged -= value;
	}

	public event Action<IEntity, IEntity, IEntity> ParentChanged
	{
		add => Hierarchy.ParentChanged += value;
		remove => Hierarchy.ParentChanged -= value;
	}

	public event Action<IEntity, int, int> ParentIndexChanged
	{
		add => Hierarchy.ParentIndexChanged += value;
		remove => Hierarchy.ParentIndexChanged -= value;
	}

	public event Action<IEntity> ChildrenChanged
	{
		add => Hierarchy.ChildrenChanged += value;
		remove => Hierarchy.ChildrenChanged -= value;
	}

	public event Action<IEntity, IEntity, int> ChildAdded
	{
		add => Hierarchy.ChildAdded += value;
		remove => Hierarchy.ChildAdded -= value;
	}

	public event Action<IEntity, IEntity, int> ChildRemoved
	{
		add => Hierarchy.ChildRemoved += value;
		remove => Hierarchy.ChildRemoved -= value;
	}
	#endregion

	#region Sleeping
	public event Action<IEntity, int, int> SleepingChanged
	{
		add => Sleep.SleepingChanged += value;
		remove => Sleep.SleepingChanged -= value;
	}
	#endregion

	public event Action<IEntity, bool, bool> IsAutoDisposableChanged
	{
		add => AutoDispose.IsAutoDisposableChanged += value;
		remove => AutoDispose.IsAutoDisposableChanged -= value;
	}

	public event Action<IEntity, IComponent, Type> ComponentAdded;
	public event Action<IEntity, IComponent, Type> ComponentRemoved;
	public event Action<IEntity, string, string> GlobalNameChanged;
	public event Action<IEntity, string, string> LocalNameChanged;
	public event Action<IEntity, int, int> SelfSleepingChanged;
	#endregion

	#region Fields
	private readonly Dictionary<Type, IComponent> components = new();
	private string globalName = UniqueName;
	private string localName = UniqueName;
	private int selfSleeping = 0;
	private readonly EngineManager<IEntity> EngineManager;
	private readonly AutoDispose<IEntity> AutoDispose;
	private readonly Hierarchy<IEntity> Hierarchy;
	private readonly Sleep<IEntity> Sleep;
	#endregion

	#region Construct / Dispose
	public AtlasEntity()
	{
		EngineManager = new EngineManager<IEntity>(this);
		AutoDispose = new AutoDispose<IEntity>(this, () => Hierarchy.Parent == null);
		Hierarchy = new Hierarchy<IEntity>(this);
		Sleep = new Sleep<IEntity>(this);

		SetHierarchyEvents();
	}

	public AtlasEntity(bool isRoot) : this() => Hierarchy.IsRoot = isRoot;

	public AtlasEntity(string localName = null, string globalName = null) : this() { SetNames(localName, globalName); }

	public void Dispose()
	{
		Hierarchy.Dispose();
		EngineManager.Dispose();
		AutoDispose.Dispose();
		Sleep.Dispose();

		SetHierarchyEvents();

		ComponentAdded = null;
		ComponentRemoved = null;
		RemoveComponents();

		GlobalNameChanged = null;
		GlobalName = UniqueName;

		LocalNameChanged = null;
		LocalName = UniqueName;

		SelfSleepingChanged = null;
		SelfSleeping = 0;

		PoolManager.Instance.Put(this);
	}

	private void SetHierarchyEvents()
	{
		Hierarchy.ChildAdded += OnChildAdded;
		Hierarchy.ParentChanged += OnParentChanged;
		Hierarchy.RootChanged += OnRootChanged;
	}
	#endregion

	#region Names
	public string GlobalName
	{
		get => globalName;
		set
		{
			if(!IsValidName(globalName, value, IsRoot, n => Engine?.Entities.Has(n) ?? false))
				return;
			string previous = globalName;
			globalName = value;
			GlobalNameChanged?.Invoke(this, value, previous);
		}
	}

	[JsonProperty(PropertyName = nameof(GlobalName), Order = int.MinValue)]
	private string SerializeGlobalName
	{
		get => GlobalName;
		set
		{
			if(value == RootName)
				IsRoot = true;
			GlobalName = value;
		}
	}

	[JsonProperty(Order = int.MinValue + 1)]
	public string LocalName
	{
		get => localName;
		set
		{
			if(!IsValidName(localName, value, IsRoot, n => Parent?.HasChild(n) ?? false))
				return;
			string previous = localName;
			localName = value;
			LocalNameChanged?.Invoke(this, value, previous);
		}
	}

	public void SetNames(string name) => SetNames(name, name);

	public void SetNames(string localName, string globalName) { LocalName = localName; GlobalName = globalName; }

	public override string ToString() => GlobalName;
	#endregion

	#region Engine
	public IEngine Engine
	{
		get => EngineManager.Engine;
		set => EngineManager.Engine = value;
	}
	#endregion

	#region Hierarchy
	#region Root
	public IEntity Root => Hierarchy.Root;

	public bool IsRoot
	{
		get => Hierarchy.IsRoot;
		set => Hierarchy.IsRoot = value;
	}
	#endregion

	#region Parent
	public IEntity Parent
	{
		get => Hierarchy.Parent;
		set => Hierarchy.Parent = value;
	}

	public int ParentIndex
	{
		get => Hierarchy.ParentIndex;
		set => Hierarchy.ParentIndex = value;
	}

	public IEntity SetParent(IEntity entity) => Hierarchy.SetParent(entity);

	public IEntity SetParent(IEntity entity, int index) => Hierarchy.SetParent(entity, index);
	#endregion

	#region Get
	public IReadOnlyLinkList<IEntity> Children => Hierarchy.Children;

	[JsonProperty(PropertyName = nameof(Children), Order = int.MaxValue)]
	[ExcludeFromCodeCoverage]
	private IEnumerable<IEntity> SerializeChildren
	{
		get => Children;
		set
		{
			foreach(var child in value)
				AddChild(child);
		}
	}

	public IEnumerator<IEntity> GetEnumerator() => Hierarchy.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public IEntity this[int index]
	{
		get => Hierarchy[index];
		set => Hierarchy[index] = value;
	}

	public IEntity GetChild(int index) => Hierarchy.GetChild(index);

	public int GetChildIndex(IEntity child) => Hierarchy.GetChildIndex(child);
	#endregion

	#region Set
	public bool SetChildIndex(IEntity child, int index) => Hierarchy.SetChildIndex(child, index);

	public bool SwapChildren(int index1, int index2) => Hierarchy.SwapChildren(index1, index2);

	public bool SwapChildren(IEntity child1, IEntity child2) => Hierarchy.SwapChildren(child1, child2);
	#endregion

	#region Add
	public IEntity AddChild(IEntity child) => Hierarchy.AddChild(child);

	public IEntity AddChild(IEntity child, int index) => Hierarchy.AddChild(child, index);
	#endregion

	#region Remove
	public IEntity RemoveChild(IEntity child) => Hierarchy.RemoveChild(child);

	public IEntity RemoveChild(int index) => Hierarchy.RemoveChild(index);

	public bool RemoveChildren() => Hierarchy.RemoveChildren();
	#endregion

	#region Has
	public bool HasChild(IEntity child) => Hierarchy.HasChild(child);

	public bool HasSibling(IEntity sibling) => Hierarchy.HasSibling(sibling);

	public bool HasDescendant(IEntity descendant) => Hierarchy.HasDescendant(descendant);

	public bool HasAncestor(IEntity ancestor) => Hierarchy.HasAncestor(ancestor);
	#endregion
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
		where TComponent : class, IComponent, new() => AddComponent(PoolManager.Instance.Get<TComponent>(), type);

	public TComponent AddComponent<TComponent>(TComponent component, int index)
		where TComponent : class, IComponent => AddComponent(component, null, index);

	public TComponent AddComponent<TComponent>(TComponent component, Type type = null, int? index = null)
		where TComponent : class, IComponent
	{
		if(component.HasManager(this))
			return component;

		type = AtlasComponent.GetType(component, type ?? typeof(TComponent));
		if(components.TryGetValue(type, out var current))
		{
			if(component == current)
				return component;
			RemoveComponent<IComponent>(type);
		}
		components.Add(type, component);
		component.AddManager(this, type, index);
		ComponentAdded?.Invoke(this, component, type);
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
		ComponentRemoved?.Invoke(this, component, type);
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
			SelfSleepingChanged?.Invoke(this, value, previous);
			if(Parent == null)
				return;
			if(value > 0 && previous <= 0)
			{
				Parent.SleepingChanged -= OnParentSleepingChanged;
				if(Parent.IsSleeping)
					--Sleeping;
			}
			else if(value <= 0 && previous > 0)
			{
				Parent.SleepingChanged += OnParentSleepingChanged;
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
	[JsonProperty(Order = int.MinValue + 2)]
	public bool IsAutoDisposable
	{
		get => AutoDispose.IsAutoDisposable;
		set => AutoDispose.IsAutoDisposable = value;
	}
	#endregion

	#region Listeners
	private void OnRootChanged(IEntity entity, IEntity current, IEntity previous)
	{
		if(previous == this)
		{
			string uniqueName = UniqueName;
			GlobalName = uniqueName;
			LocalName = uniqueName;
		}
		else if(current == this)
		{
			GlobalName = RootName;
			LocalName = RootName;
		}
	}

	private void OnChildAdded(IEntity parent, IEntity entity, int index)
	{
		foreach(var child in parent.Children)
		{
			if(child == entity || child.LocalName != entity.LocalName)
				continue;
			entity.LocalName = UniqueName;
			return;
		}
	}

	private void OnParentChanged(IEntity entity, IEntity current, IEntity previous)
	{
		SetSleeping(current, previous);
		AutoDispose.TryAutoDispose();
	}

	private void SetSleeping(IEntity current, IEntity previous)
	{
		if(IsSelfSleeping)
			return;

		int deltaSleeping = 0;

		if(previous != null)
		{
			if(previous.IsSleeping)
				--deltaSleeping;
			previous.SleepingChanged -= OnParentSleepingChanged;
		}

		if(current != null)
		{
			if(current.IsSleeping)
				++deltaSleeping;
			current.SleepingChanged += OnParentSleepingChanged;
		}

		Sleeping += deltaSleeping;
	}

	private void OnParentSleepingChanged(IEntity parent, int current, int previous)
	{
		if(current > 0 && previous <= 0)
			++Sleeping;
		else if(current <= 0 && previous > 0)
			--Sleeping;
	}
	#endregion
}