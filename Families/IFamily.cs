using Atlas.Components;
using Atlas.Engine;
using Atlas.Entities;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Families
{
	interface IFamily:IEngine<IFamily>, IDispose
	{
		IFamilyType FamilyType { get; }

		IReadOnlyLinkList<IEntity> Entities { get; }

		void AddEntity(IEntity entity);
		void RemoveEntity(IEntity entity);

		void AddEntity(IEntity entity, IComponent component, Type type);
		void RemoveEntity(IEntity entity, IComponent component, Type type);

		ISignal<IFamily, IEntity> EntityAdded { get; }
		ISignal<IFamily, IEntity> EntityRemoved { get; }
	}
}