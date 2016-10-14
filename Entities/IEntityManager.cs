using Atlas.Components;
using Atlas.LinkList;
using Atlas.Signals;

namespace Atlas.Entities
{
	interface IEntityManager:IComponent
	{
		new IReadOnlyLinkList<IEntity> Entities { get; }

		bool HasEntity(string globalName);
		bool HasEntity(IEntity entity);

		IEntity GetEntity(string globalName);

		new Signal<IEntityManager, IEntity> EntityAdded { get; }
		new Signal<IEntityManager, IEntity> EntityRemoved { get; }
	}
}
