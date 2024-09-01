using System;

namespace Atlas.ECS.Components.Engine;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IEngine"/> management.
/// </summary>
public interface IReadOnlyEngineManager
{
	/// <summary>
	/// The <see cref="IEngine"/> instance.
	/// </summary>
	IEngine Engine { get; }
}

/// <summary>
/// An <see langword="interface"/> providing <see cref="IEngine"/> management.
/// </summary>
public interface IEngineManager<out T> : IReadOnlyEngineManager where T : IEngineManager<T>
{
	/// <summary>
	/// The <see langword="event"/> invoked when the <see cref="IEngine"/> has changed.
	/// </summary>
	event Action<T, IEngine, IEngine> EngineChanged;

	/// <summary>
	/// The <see cref="IEngine"/> instance.
	/// </summary>
	new IEngine Engine { get; set; }
}