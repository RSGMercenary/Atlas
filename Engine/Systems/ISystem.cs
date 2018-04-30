using Atlas.Engine.Engine;
using Atlas.Engine.Interfaces;

namespace Atlas.Engine.Systems
{
	public interface ISystem : IEngineObject, IPriority, ISleepEngineObject, IEngineUpdate
	{

	}
}
