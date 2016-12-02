using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Interfaces;
using Atlas.Engine.Signals;
using System;

namespace Atlas.Engine.Components
{
	interface IComponent:IReadOnlyEngineObject<IComponent>, IReset
	{
		int GetEntityIndex(IEntity entity);
		bool SetEntityIndex(IEntity entity, int index);

		IEntity AddEntity(IEntity entity);
		IEntity AddEntity(IEntity entity, Type type);
		IEntity AddEntity(IEntity entity, int index);
		IEntity AddEntity(IEntity entity, Type type = null, int index = int.MaxValue);

		IEntity RemoveEntity(IEntity entity);
		IEntity RemoveEntity(IEntity entity, Type type);
		IEntity RemoveEntity(int index);
		bool RemoveEntities();

		ISignal<IComponent, IEntity, int> EntityAdded { get; }
		ISignal<IComponent, IEntity, int> EntityRemoved { get; }

		IEntity Entity { get; }
		IReadOnlyLinkList<IEntity> Entities { get; }

		bool IsShareable { get; }

		string ToString(bool addEntities = true, int index = 0, string indent = "");
	}

	interface IComponent<TBaseAbstraction>:IComponent where TBaseAbstraction : IComponent
	{
		IEntity AddEntity<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction;
		IEntity AddEntity<TAbstraction>(IEntity entity, int index) where TAbstraction : TBaseAbstraction;

		IEntity RemoveEntity<TAbstraction>(IEntity entity) where TAbstraction : TBaseAbstraction;
	}
}
