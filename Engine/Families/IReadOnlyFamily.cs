using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Entities;

namespace Atlas.Engine.Families
{
	public interface IReadOnlyFamily : IEngineObject
	{
		IReadOnlyEngineList<IEntity> Entities { get; }
	}
}
