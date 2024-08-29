using Atlas.Core.Collections.LinkList;
using Atlas.Core.Extensions;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Systems;

internal class SystemManager : ISystemManager
{
	public event Action<ISystemManager, ISystem, Type> Added;
	public event Action<ISystemManager, ISystem, Type> Removed;

	private readonly LinkList<ISystem> systems = new();
	private readonly Dictionary<Type, ISystem> types = new();
	private readonly Dictionary<Type, int> references = new();

	public IEngine Engine { get; }

	public SystemManager(IEngine engine)
	{
		Engine = engine;
	}

	#region Create
	public ISystemCreator Creator { get; set; }

	private TSystem CreateSystem<TSystem>() where TSystem : class, ISystem => Creator != null ? Creator.Create<TSystem>() : SystemGetter.GetSystem<TSystem>();

	private T GenericInvoke<T>(Type type, string name)
	{
		if(!typeof(ISystem).IsAssignableFrom(type))
			throw new ArgumentException($"{type} is not assignable to {typeof(ISystem)}.");
		return (T)GetType()
			.GetMethod(name, 1, [])
			.MakeGenericMethod(type)
			.Invoke(this, null);
	}
	#endregion

	#region Add
	public TSystem Add<TSystem>() where TSystem : class, ISystem
	{
		var type = typeof(TSystem);
		if(!types.TryGetValue(type, out TSystem system))
		{
			system = CreateSystem<TSystem>();

			system.PriorityChanged += PriorityChanged;
			PriorityChanged(system);

			types.Add(type, system);
			references.Add(type, 0);
			system.Engine = Engine;

			Added?.Invoke(this, system, type);
		}
		++references[type];
		return (TSystem)types[type];
	}

	public ISystem Add(Type type) => GenericInvoke<ISystem>(type, nameof(Add));
	#endregion

	#region Remove
	public bool Remove<TSystem>() where TSystem : class, ISystem
	{
		var type = typeof(TSystem);
		if(!types.TryGetValue(type, out var system))
			return false;
		if(--references[type] > 0)
			return false;

		system.PriorityChanged -= PriorityChanged;
		systems.Remove(system);

		types.Remove(type);
		references.Remove(type);
		system.Engine = null;

		Removed?.Invoke(this, system, type);
		return true;
	}

	public bool Remove(Type type) => GenericInvoke<bool>(type, nameof(Remove));
	#endregion

	#region Get
	[JsonIgnore]
	public IReadOnlyLinkList<ISystem> Systems => systems;

	public IReadOnlyDictionary<Type, ISystem> Types => types;

	public TSystem Get<TSystem>() where TSystem : class, ISystem => (TSystem)Get(typeof(TSystem));

	public ISystem Get(Type type) => types.TryGetValue(type, out var system) ? system : null;

	public ISystem Get(int index) => systems[index];
	#endregion

	#region Has
	public bool Has<TSystem>() where TSystem : class, ISystem => Has(typeof(TSystem));

	public bool Has(Type type) => types.ContainsKey(type);

	public bool Has(ISystem system) => systems.Contains(system);
	#endregion

	#region Messages
	private void PriorityChanged(ISystem system, int currentIndex = -1, int previousIndex = -1)
	{
		var node = systems.GetNode(system);
		var index = systems.Count - 1;
		for(var current = systems.Last; current != null; current = current.Previous)
		{
			if(current.Value.Priority <= system.Priority)
			{
				systems.SetNode(node, index);
				return;
			}
			--index;
		}
		systems.Add(system, 0);
	}
	#endregion
}