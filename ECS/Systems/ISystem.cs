using Atlas.ECS.Objects;

namespace Atlas.ECS.Systems
{
	public interface IReadOnlySystem : ISleepObject, IUpdatePhaseObject, IEngineObject
	{
		int Priority { get; }
	}

	public interface ISystem : IReadOnlySystem
	{
		void FixedUpdate(double deltaTime);
		void Update(double deltaTime);
	}
}
