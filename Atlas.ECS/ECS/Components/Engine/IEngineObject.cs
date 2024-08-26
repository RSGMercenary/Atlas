using System;

namespace Atlas.ECS.Components.Engine;

public interface IReadOnlyEngineObject
{
	IEngine Engine { get; }
}

public interface IEngineObject<T> : IReadOnlyEngineObject where T : IEngineObject<T>
{
	event Action<T, IEngine, IEngine> EngineChanged;

	new IEngine Engine { get; set; }
}