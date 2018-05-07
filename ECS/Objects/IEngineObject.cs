using Atlas.ECS.Components;
using Atlas.Framework.Objects;

namespace Atlas.ECS.Objects
{
	public interface IEngineObject : IObject
	{
		IEngine Engine { get; set; }
	}

	public interface IEngineObject<T> : IEngineObject, IObject<T>
		where T : IEngineObject<T>
	{

	}
}
