using Atlas.Entities;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Components
{
	interface IComponent:IDispose
	{
		int GetManagerIndex(IEntity entity);
		bool SetManagerIndex(IEntity entity, int index);

		IEntity AddManager(IEntity entity);
		IEntity AddManager(IEntity entity, Type type);
		IEntity AddManager(IEntity entity, int index);
		IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue);

		IEntity RemoveManager(IEntity entity);
		IEntity RemoveManager(int index);
		bool RemoveManagers();

		Signal<IComponent, IEntity, int> ManagerAdded { get; }
		Signal<IComponent, IEntity, int> ManagerRemoved { get; }

		IEntity Manager { get; }
		IReadOnlyLinkList<IEntity> Managers { get; }

		bool IsShareable { get; }
	}
}
