using Atlas.ECS.Components;
using Atlas.Core.Objects;

namespace Atlas.ECS.Objects
{
	public interface IEngineObject : IObject
	{
		IEngine Engine { get; set; }
	}
}
