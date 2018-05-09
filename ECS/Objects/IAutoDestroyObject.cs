using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IAutoDestroyObject : IObject
	{
		bool AutoDestroy { get; set; }
	}
}
