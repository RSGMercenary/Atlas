using Atlas.Core.Objects;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface ISystem : IObject, ISleep, IUpdate
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
		float DeltaIntervalTime { get; }

		float TotalIntervalTime { get; }

		/// <summary>
		/// Determines whether the System is fixed-time, variable-time, or event-based.
		/// </summary>
		TimeStep TimeStep { get; }
	}
}
