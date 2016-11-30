using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Components;
using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Families
{
	interface IFamily:IEngineObject<IFamily>
	{
		Type FamilyType { get; set; }

		IReadOnlyLinkList<IEntity> Entities { get; }

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, IComponent component, Type type);
		void RemoveEntity(IEntity entity, IComponent component, Type type);

		ISignal<IFamily, IEntity> EntityAdded { get; }
		ISignal<IFamily, IEntity> EntityRemoved { get; }
	}
}