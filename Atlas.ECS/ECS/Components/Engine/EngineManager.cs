using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine;
public class EngineManager<T> : IEngineManager<T>, IDisposable where T : IEngineManager<T>
{
	public event Action<T, IEngine, IEngine> EngineChanged;

	private T Instance { get; }

	public EngineManager(T instance)
	{
		Instance = instance;
	}

	public void Dispose()
	{
		EngineChanged = null;
	}

	public IEngine Engine
	{
		get => field;
		set
		{
			if(!(value != null && field == null && HasEngineManager(value)) &&
				!(value == null && field != null && !HasEngineManager(field)))
				return;
			var previous = field;
			field = value;
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