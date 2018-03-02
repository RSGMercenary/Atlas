using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	public interface ISystem : IEngineObject, IPriority, ISleep, IEngineUpdate
	{

	}
}
