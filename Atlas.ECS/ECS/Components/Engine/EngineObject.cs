using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine;
public class EngineObject<T> : IEngineObject<T> where T : IEngineObject<T>
{
	public event Action<T, IEngine, IEngine> EngineChanged;

	private IEngine engine;
	private readonly T Instance;
	private readonly Action<IEngine, IEngine> Changed;

	public EngineObject(T instance, Action<IEngine, IEngine> changed = null)
	{
		Instance = instance;
		Changed = changed;
	}


	public IEngine Engine
	{
		get => engine;
		set
		{
			if(!(value != null && engine == null && HasEngineObject(value)) &&
				!(value == null && engine != null && !HasEngineObject(engine)))
				return;
			var previous = engine;
			engine = value;
			Changed?.Invoke(value, previous);
			EngineChanged?.Invoke(Instance, value, previous);
		}
	}

	private bool HasEngineObject(IEngine engine)
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