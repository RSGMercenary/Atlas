using Atlas.Core.Objects.Sleep;
using Atlas.Core.Objects.Update;
using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface ISystem : IObject<ISystem>, IUpdate<float>, ISleep, IUpdateState
	{
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
		TimeStep UpdateStep { get; }
	}
}