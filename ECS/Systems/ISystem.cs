using Atlas.Core.Objects;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface IReadOnlySystem : IObject, ISleep, IUpdateState
	{
		/// <summary>
		/// The Priority of this System relative to other Systems in the Engine.
		/// Systems are updated from lowest-to-highest Priority value.
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// The delay between updates. This is useful for Systems that don't have to
		/// update every loop. Systems will update once the Interval has been reached.
		/// </summary>
		double DeltaIntervalTime { get; }

		double TotalIntervalTime { get; }

		/// <summary>
		/// Determines whether the System is fixed-time, variable-time, or is event-based.
		/// </summary>
		TimeStep TimeStep { get; }
	}

	public interface IReadOnlySystem<T> : IReadOnlySystem, IObject<T>, ISleep<T>, IUpdateState<T>
		where T : IReadOnlySystem
	{
	}

	public interface ISystem : IReadOnlySystem
	{
		void Update(double deltaTime);
	}

	public interface ISystem<T> : ISystem, IReadOnlySystem<T>
		where T : ISystem
	{
	}
}
