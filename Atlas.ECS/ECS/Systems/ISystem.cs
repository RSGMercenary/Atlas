using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Serialization;
using System;

namespace Atlas.ECS.Systems;

public interface ISystem : IEngineManager<ISystem>, IUpdate<float>, ISleep<ISystem>, IDisposable, ISerialize
{
	event Action<ISystem, UpdatePhase, UpdatePhase> UpdatePhaseChanged;
	event Action<ISystem, TimeStep, TimeStep> TimeStepChanged;
	event Action<ISystem, float, float> IntervalChanged;
	event Action<ISystem, int, int> PriorityChanged;

	/// <summary>
	/// Automatically called on Systems removed from the Engine.
	/// </summary>
	new void Dispose();

	int Priority { get; set; }

	/// <summary>
	/// The delay between updates. This is useful for Systems that don't have to
	/// update every loop. Systems will update once the Interval has been reached.
	/// </summary>
	float DeltaIntervalTime { get; }

	float TotalIntervalTime { get; }

	/// <summary>
	/// Determines whether the System is fixed-time, variable-time, or event-based.
	/// </summary>
	TimeStep TimeStep { get; }
}