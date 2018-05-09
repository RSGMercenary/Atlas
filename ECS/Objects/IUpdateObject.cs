using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IUpdateObject : IObject
	{
		bool IsUpdating { get; }
	}
}
