using Atlas.Core.Objects;

namespace Atlas.ECS.Objects
{
	public interface IUpdateObject : IObject
	{
		bool IsUpdating { get; }
	}
}
