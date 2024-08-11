using Atlas.Core.Messages;
using Atlas.ECS.Entities;
using Atlas.ECS.Families;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine;

internal class EngineItem<T> : IEngineItem
	where T : IEngineItem, IMessenger<T>
{
	private readonly T Instance;
	private readonly Action<IEngine, IEngine> Listener;
	private IEngine engine;

	public EngineItem(T instance, Action<IEngine, IEngine> listener = null)
	{
		Instance = instance;
		Listener = listener;
	}

	public IEngine Engine
	{
		get => engine;
		set
		{
			if(!(value != null && engine == null && HasEngineItem(value)) &&
				!(value == null && engine != null && !HasEngineItem(engine)))
				return;
			var previous = engine;
			engine = value;
			Listener?.Invoke(value, previous);
			Instance.Message<IEngineMessage<T>>(new EngineMessage<T>(value, previous));
		}
	}

	private bool HasEngineItem(IEngine engine)
	{
		if(Instance is IEntity entity)
			return engine.HasEntity(entity);
		if(Instance is IFamily family)
			return engine.HasFamily(family);
		if(Instance is ISystem system)
			return engine.HasSystem(system);
		return false;
	}
}