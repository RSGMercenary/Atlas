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

	private readonly Group<ISystem> list = new();
	private readonly Dictionary<Type, ISystem> dictionary = new();
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
		if(!dictionary.TryGetValue(type, out var system))
		{
			system = CreateSystem(type);
			system.PriorityChanged += PriorityChanged;
			PriorityChanged(system);
			dictionary.Add(type, system);
			references.Add(type, 0);
			system.Engine = Engine;

			Added?.Invoke(this, system, type);
		}
		++references[type];
		return dictionary[type];
	}
	#endregion

	#region emove
	public bool Remove<TSystem>() where TSystem : class, ISystem => Remove(typeof(TSystem));

	public bool Remove(Type type)
	{
		if(!dictionary.TryGetValue(type, out var system))
			return false;
		if(--references[type] > 0)
			return false;
		system.PriorityChanged -= PriorityChanged;
		list.Remove(system);
		dictionary.Remove(type);
		references.Remove(type);
		system.Engine = null;

		Removed?.Invoke(this, system, type);
		return true;
	}
	#endregion

	#region Get
	[JsonIgnore]
	public IReadOnlyGroup<ISystem> Systems => list;

	public TISystem Get<TISystem>() where TISystem : ISystem => (TISystem)Get(typeof(TISystem));

	public ISystem Get(Type type) => dictionary.TryGetValue(type, out var system) ? system : null;

	public ISystem Get(int index) => list[index];
	#endregion

	#region Has
	public bool Has(ISystem system) => list.Contains(system);

	public bool Has<TISystem>() where TISystem : ISystem => Has(typeof(TISystem));

	public bool Has(Type type) => dictionary.ContainsKey(type);
	#endregion

	#region Messages
	private void PriorityChanged(ISystem system, int current = -1, int previous = -1)
	{
		list.Remove(system);
		for(var index = list.Count; index > 0; --index)
		{
			if(list[index - 1].Priority <= system.Priority)
			{
				list.Insert(index, system);
				return;
			}
		}
		list.Insert(0, system);
	}
	#endregion
}