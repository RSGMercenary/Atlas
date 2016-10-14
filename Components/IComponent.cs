using Atlas.Entities;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Components
{
	interface IComponent:IDispose
	{
		int GetEntityIndex(IEntity entity);
		bool SetEntityIndex(IEntity entity, int index);

		IEntity AddEntity(IEntity entity);
		IEntity AddEntity(IEntity entity, Type type);
		IEntity AddEntity(IEntity entity, int index);
		IEntity AddEntity(IEntity entity, Type type, int index);
		Signal<IComponent, IEntity, int> EntityAdded { get; }

		IEntity RemoveEntity(IEntity entity);
		IEntity RemoveEntity(int index);
		void RemoveComponentManagers();
		Signal<IComponent, IEntity, int> EntityRemoved { get; }

		IEntity Entity { get; }
		IReadOnlyLinkList<IEntity> Entities { get; }

		bool IsShareable { get; }
	}
}
