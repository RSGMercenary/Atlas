using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IUpdatePhaseObject : IObject
	{
		UpdatePhase UpdateState { get; }
	}
}
