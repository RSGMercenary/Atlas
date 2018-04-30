using Atlas.Engine.Collections.EngineList;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Families
{
	public interface IFamily : IEngineObject
	{
		IReadOnlyEngineList<IFamilyMember> Members { get; }

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);
	}
}
