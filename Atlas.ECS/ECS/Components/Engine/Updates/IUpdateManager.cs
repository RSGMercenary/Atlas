using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

public interface IUpdateManager : IReadOnlyEngineManager, IUpdater<IUpdateManager>, IUpdate<float>
{
	#region Events
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="DeltaFixedTime"/> has changed.
	/// </summary>
	event Action<IUpdateManager, float, float> DeltaFixedTimeChanged;

	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="MaxVariableTime"/> has changed.
	/// </summary>
	event Action<IUpdateManager, float, float> MaxVariableTimeChanged;
	#endregion

	#region Fixed Time
	/// <summary>
	/// The delta time between fixed-time updates. This is set manually.
	/// </summary>
	float DeltaFixedTime { get; set; }

	/// <summary>
	/// The total time running fixed-time updates. This is set every loop.
	/// </summary>
	float TotalFixedTime { get; }

	int FixedLag { get; }

	int FixedUpdates { get; }
	#endregion

	#region Variable Time
	/// <summary>
	/// The delta time between variable-time updates. This is set every loop.
	/// </summary>
	float DeltaVariableTime { get; }

	/// <summary>
	/// The total time running variable-time updates. This is set every loop.
	/// </summary>
	float TotalVariableTime { get; }

	/// <summary>
	/// The max delta time between updates. Prevents the update loop "spiral of death".
	/// </summary>
	float MaxVariableTime { get; set; }

	float VariableInterpolation { get; }
	#endregion

	#region Updates
	/// <summary>
	/// The current System that's undergoing an Update().
	/// </summary>
	ISystem UpdateSystem { get; }
	#endregion
}