using Atlas.Core.Collections.Group;
using Atlas.ECS.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Atlas.ECS.Components.Engine.Systems;

internal class SystemManager : ISystemManager
{
	public event Action<ISystemManager, ISystem, Type> Added;
	public event Action<ISystemManager, ISystem, Type> Removed;

	private readonly Group<ISystem> systems = new();
	private readonly Dictionary<Type, ISystem> types = new();
	private readonly Dictionary<Type, int> references = new();

	public IEngine Engine { get; }

	public SystemManager(IEngine engine)
	{
		Engine = engine;
	}

	#region Create
	protected virtual ISystem CreateSystem(Type type) => SystemGetter.GetSystem(type);
	#endregion

	#region Add
	public TSystem Add<TSystem>() where TSystem : class, ISystem => (TSystem)Add(typeof(TSystem));

	public ISystem Add(Type type)
	{
		if(!types.TryGetValue(type, out var system))
		{
			system = CreateSystem(type);
			system.PriorityChanged += PriorityChanged;
			PriorityChanged(system);
			types.Add(type, system);
			references.Add(type, 0);
			system.Engine = Engine;

			Added?.Invoke(this, system, type);
		}
		++references[type];
		return types[type];
	}
	#endregion

	#region Remove
	public bool Remove<TSystem>() where TSystem : class, ISystem => Remove(typeof(TSystem));

	public bool Remove(Type type)
	{
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
	#endregion

	#region Get
	[JsonIgnore]
	public IReadOnlyGroup<ISystem> Systems => systems;

	public IReadOnlyDictionary<Type, ISystem> Types => types;

	public TISystem Get<TISystem>() where TISystem : ISystem => (TISystem)Get(typeof(TISystem));

	public ISystem Get(Type type) => types.TryGetValue(type, out var system) ? system : null;

	public ISystem Get(int index) => systems[index];
	#endregion

	#region Has
	public bool Has(ISystem system) => systems.Contains(system);

	public bool Has<TISystem>() where TISystem : ISystem => Has(typeof(TISystem));

	public bool Has(Type type) => types.ContainsKey(type);
	#endregion

	#region Messages
	private void PriorityChanged(ISystem system, int current = -1, int previous = -1)
	{
		systems.Remove(system);
		for(var index = systems.Count; index > 0; --index)
		{
			if(systems[index - 1].Priority <= system.Priority)
			{
				systems.Insert(index, system);
				return;
			}
		}
		systems.Insert(0, system);
	}
	#endregion
}