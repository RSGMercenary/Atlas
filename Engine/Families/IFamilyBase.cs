using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;

namespace Atlas.Engine.Families
{
	public interface IFamilyBase : IEngineObject
	{
		IReadOnlyEngineList<IEntity> Entities { get; }
	}
}
