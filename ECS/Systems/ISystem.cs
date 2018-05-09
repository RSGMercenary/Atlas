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
		/// Determines whether a System receives an Update(deltaFixedTime)
		/// or Update(deltaTime) during an update loop.
		/// </summary>
		bool IsFixed { get; }
	}

	public interface ISystem : IReadOnlySystem
	{
		void Update(double deltaTime);
	}
}
