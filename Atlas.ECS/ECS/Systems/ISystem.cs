using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Components.Engine;
using Atlas.ECS.Components.Engine.Systems;
using Atlas.ECS.Serialization;
using System;

namespace Atlas.ECS.Systems;

public interface ISystem : IEngineManager<ISystem>, IUpdater<ISystem>, IUpdate<float>, ISleep<ISystem>, IDisposable, ISerialize
{
	/// <summary>
	/// The <see langword="event"/> invoked when <see cref="Priority"/> has changed.
	/// </summary>
	event Action<ISystem, int, int> PriorityChanged;

	event Action<ISystem, float, float> IntervalChanged;

	/// <summary>
	/// Automatically called on <see cref="ISystem"/> instances removed from the <see cref="ISystemManager"/>.
	/// </summary>
	new void Dispose();

	/// <summary>
	/// The <see cref="Core.Objects.Update.TimeStep"/> of the <see cref="ISystem"/>.
	/// <para><see cref="ISystem"/> instances are added to <see cref="ISystemManager.FixedSystems"/>, 
	/// <see cref="ISystemManager.VariableSystems"/>, or no list based on their <see cref="TimeStep"/>.</para>
	/// </summary>
	new TimeStep TimeStep { get; }

	/// <summary>
	/// The <see cref="Priority"/> of the <see cref="ISystem"/>.
	/// <para><see cref="ISystem"/> instances are ordered and updated in <see cref="ISystemManager"/> based on their <see cref="Priority"/>.</para>
	/// </summary>
	int Priority { get; set; }

	/// <summary>
	/// The delay between updates. This is useful for Systems that don't have to
	/// update every loop. Systems will update once the Interval has been reached.
	/// </summary>
	float DeltaIntervalTime { get; }

	float TotalIntervalTime { get; }
}