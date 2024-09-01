using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Systems;
using System;

namespace Atlas.ECS.Components.Engine.Updates;

/// <summary>
/// An <see langword="interface"/> providing <see cref="IUpdate{T}.Update(T)"/> management.
/// </summary>
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
	/// The delta time used with <see cref="ISystemManager.FixedSystems"/> during the <see cref="IUpdate{T}.Update(T)"/> loop.
	/// </summary>
	float DeltaFixedTime { get; set; }

	/// <summary>
	/// The total time running <see cref="IUpdateManager"/> fixed-time updates.
	/// </summary>
	float TotalFixedTime { get; }

	/// <summary>
	/// 
	/// </summary>
	int FixedLag { get; }

	/// <summary>
	/// The amount of fixed-time updates to match <see cref="DeltaVariableTime"/> during the <see cref="IUpdate{T}.Update(T)"/> loop.
	/// </summary>
	int FixedUpdates { get; }
	#endregion

	#region Variable Time
	/// <summary>
	/// The delta time used with <see cref="ISystemManager.VariableSystems"/> during the <see cref="IUpdate{T}.Update(T)"/> loop.
	/// </summary>
	float DeltaVariableTime { get; }

	/// <summary>
	/// The total time running <see cref="IUpdateManager"/> variable-time updates.
	/// </summary>
	float TotalVariableTime { get; }

	/// <summary>
	/// The max delta time <see cref="DeltaVariableTime"/> can be during variable-time updates.
	/// <para>Prevents the update loop "spiral of death" where updates can't catch up to elapsed time.</para>
	/// </summary>
	float MaxVariableTime { get; set; }

	/// <summary>
	/// <para>Interpolation: (<see cref="TotalVariableTime"/> - <see cref="TotalFixedTime"/>) / <see cref="DeltaFixedTime"/></para>
	/// </summary>
	float VariableInterpolation { get; }
	#endregion

	#region Updates
	/// <summary>
	/// The <see cref="ISystem"/> being updated during the <see cref="IUpdate{T}.Update(T)"/> loop.
	/// </summary>
	ISystem UpdateSystem { get; }

	/// <summary>
	/// The <see cref="Core.Objects.Update.TimeStep"/> of the <see cref="IUpdateManager"/> during the <see cref="IUpdate{T}.Update(T)"/> loop.
	/// </summary>
	new TimeStep TimeStep { get; }

	new void Update(float deltaTime);
	#endregion
}