using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IUpdatePhaseObject : IObject
	{
		UpdatePhase UpdateState { get; }
	}

	public interface IUpdatePhaseObject<T> : IUpdatePhaseObject, IObject<T>
		where T : IUpdatePhaseObject<T>
	{

	}
}
