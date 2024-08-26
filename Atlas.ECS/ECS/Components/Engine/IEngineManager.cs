using System;

namespace Atlas.ECS.Components.Engine;

public interface IReadOnlyEngineManager
{
	IEngine Engine { get; }
}

public interface IEngineManager<T> : IReadOnlyEngineManager where T : IEngineManager<T>
{
	event Action<T, IEngine, IEngine> EngineChanged;

	new IEngine Engine { get; set; }
}