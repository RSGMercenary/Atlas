using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Components
{
	interface IComponent:IReadOnlyEngineObject<IComponent>, IBaseObject<IComponent>, IReset
	{
		int GetManagerIndex(IEntity entity);
		bool SetManagerIndex(IEntity entity, int index);

		bool SwapManagers(IEntity entity1, IEntity entity2);
		bool SwapManagers(int index1, int index2);

		IEntity AddManager(IEntity entity);
		IEntity AddManager(IEntity entity, Type type);
		IEntity AddManager(IEntity entity, int index);
		IEntity AddManager(IEntity entity, Type type = null, int index = int.MaxValue);

		IEntity AddManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent;

		IEntity AddManager<TIComponent>(IEntity entity, int index = int.MaxValue)
			where TIComponent : IComponent;

		IEntity RemoveManager(IEntity entity);
		IEntity RemoveManager(IEntity entity, Type type);
		IEntity RemoveManager(int index);

		IEntity RemoveManager<TIComponent>(IEntity entity)
			where TIComponent : IComponent;

		bool RemoveManagers();

		ISignal<IComponent, IEntity, int> ManagerAdded { get; }
		ISignal<IComponent, IEntity, int> ManagerRemoved { get; }

		IEntity Manager { get; }
		IReadOnlyLinkList<IEntity> Managers { get; }

		bool IsShareable { get; }

		string ToString(bool addEntities = true, int index = 0, string indent = "");
	}
}
