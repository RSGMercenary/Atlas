using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface IReadOnlySystem : ISleepObject, IUpdateObject, IEngineObject
	{
		/// <summary>
		/// The priority of this System relative to other Systems in the Engine.
		/// Systems are ordered from lowest-to-highest Priority value.
		/// </summary>
		int Priority { get; }

		/// <summary>
		/// The fixed time of this System in seconds (1/60, 1/30, 5, etc). Systems with a fixed time get their
		/// Update(FixedTime) method called the number of times a fixed update is need for a given update loop.
		/// </summary>
		double FixedTime { get; }
	}

	public interface ISystem : IReadOnlySystem
	{
		void Update(double deltaTime);
	}
}
