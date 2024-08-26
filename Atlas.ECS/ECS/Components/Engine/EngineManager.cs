using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine;
public class EngineManager<T> : IEngineManager<T> where T : IEngineManager<T>
{
	public event Action<T, IEngine, IEngine> EngineChanged;

	private IEngine engine;
	private readonly T Instance;
	private readonly Action<IEngine, IEngine> Changed;

	public EngineManager(T instance, Action<IEngine, IEngine> changed = null)
	{
		Instance = instance;
		Changed = changed;
	}


	public IEngine Engine
	{
		get => engine;
		set
		{
			if(!(value != null && engine == null && HasEngineManager(value)) &&
				!(value == null && engine != null && !HasEngineManager(engine)))
				return;
			var previous = engine;
			engine = value;
			Changed?.Invoke(value, previous);
			EngineChanged?.Invoke(Instance, value, previous);
		}
	}

	private bool HasEngineManager(IEngine engine)
	{
		if(Instance is IEntity entity)
			return engine.Entities.Has(entity);
		if(Instance is IFamily family)
			return engine.Families.Has(family);
		if(Instance is ISystem system)
			return engine.Systems.Has(system);
		return false;
	}
}