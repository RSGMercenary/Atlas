using Atlas.ECS.Components;
using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IEngineObject : IObject
	{
		IEngine Engine { get; set; }
	}
}
