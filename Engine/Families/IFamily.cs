using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Families
{
	public interface IFamily : IEngineObject<IFamily>
	{
		Type FamilyType { get; set; }

		IReadOnlyLinkList<IEntity> Entities { get; }

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, Type type);
		void RemoveEntity(IEntity entity, Type type);

		ISignal<IFamily, IEntity> EntityAdded { get; }
		ISignal<IFamily, IEntity> EntityRemoved { get; }
	}
}