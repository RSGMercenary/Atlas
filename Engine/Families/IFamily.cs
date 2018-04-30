using Atlas.Engine.Entities;
using System;

namespace Atlas.Engine.Families
{
	public interface IFamily : IFamilyBase
	{
		Type FamilyType { get; set; }

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);
	}
}