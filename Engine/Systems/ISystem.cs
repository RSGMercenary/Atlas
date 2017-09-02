using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	public interface ISystem : IEngineObject<ISystem>, IPriority, ISleep, IEngineUpdate
	{

	}
}
