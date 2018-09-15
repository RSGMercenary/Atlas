using Atlas.Core.Objects;
using Atlas.ECS.Components;

namespace Atlas.ECS.Objects
{
	public interface IEngineObject : IObject
	{
		IEngine Engine { get; set; }
	}
}
