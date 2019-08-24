using Atlas.Core.Objects;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface ISystem : IObject<ISystem>, ISleep, IUpdateState, IPriority
	{
		void Update(float deltaTime);

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