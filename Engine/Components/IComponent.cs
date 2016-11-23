using Atlas.Engine.Engine;
using Atlas.Engine.Entities;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Engine.Components
{
	interface IComponent<TBaseAbstraction>:IComponent where TBaseAbstraction : IComponent
	{
		IEntity AddManager<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction;
		IEntity AddManager<TAbstraction>(IEntity entity, int index) where TAbstraction : TBaseAbstraction;

		IEntity RemoveManager<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction;
	}

	interface IComponent:IReadOnlyEngine<IComponent>, IDispose, IAutoDispose
	{
		int GetManagerIndex(IEntity entity);
		bool SetManagerIndex(IEntity entity, int index);

		IEntity AddManager(IEntity entity);
		IEntity AddManager(IEntity entity, Type type);
		IEntity AddManager(IEntity entity, int index);
		IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue);

		IEntity RemoveManager(IEntity entity);
		IEntity RemoveManager(IEntity entity, Type type);
		IEntity RemoveManager(int index);
		bool RemoveManagers();

		ISignal<IComponent, IEntity, int> ManagerAdded { get; }
		ISignal<IComponent, IEntity, int> ManagerRemoved { get; }

		IEntity Manager { get; }
		IReadOnlyLinkList<IEntity> Managers { get; }

		bool IsShareable { get; }
	}
}
