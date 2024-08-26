using Atlas.Core.Objects.Update;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

public interface IUpdateManager : IReadOnlyEngineObject, IUpdateState
{
	event Action<IUpdateManager, TimeStep, TimeStep> UpdateStateChanged;

	#region Variable Time
	/// <summary>
	/// The max delta time between updates. Prevents the update loop "spiral of death".
	/// </summary>
	float MaxVariableTime { get; set; }

	/// <summary>
	/// The delta time between variable-time updates. This is set every loop.
	/// </summary>
	float DeltaVariableTime { get; }

	/// <summary>
	/// The total time running variable-time updates. This is set every loop.
	/// </summary>
	float TotalVariableTime { get; }

	float VariableInterpolation { get; }
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

	#region Updates
	/// <summary>
	/// The current System that's undergoing an Update().
	/// </summary>
	ISystem UpdateSystem { get; }
	#endregion
}