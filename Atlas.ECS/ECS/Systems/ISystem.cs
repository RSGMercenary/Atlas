using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Serialization;
using System;

namespace Atlas.ECS.Systems;

public interface ISystem : IEngineManager<ISystem>, IUpdater<ISystem>, IUpdate<float>, ISleep<ISystem>, IDisposable, ISerialize
{
	event Action<ISystem, int, int> PriorityChanged;

	event Action<ISystem, float, float> IntervalChanged;

	/// <summary>
	/// Automatically called on <see cref="ISystem"/> instances removed from the <see cref="ISystemManager"/>.
	/// </summary>
	new void Dispose();

	int Priority { get; set; }

	/// <summary>
	/// The delay between updates. This is useful for Systems that don't have to
	/// update every loop. Systems will update once the Interval has been reached.
	/// </summary>
	float DeltaIntervalTime { get; }

	float TotalIntervalTime { get; }
}