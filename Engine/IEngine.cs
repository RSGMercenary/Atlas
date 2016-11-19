using Atlas.Components;
using Atlas.Entities;
using Atlas.Families;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using Atlas.Systems;
using System;

namespace Atlas.Engine
{
	interface IEngine:IComponent, IUpdate<IEngine>
	{
		IReadOnlyLinkList<IEntity> Entities { get; }
		IReadOnlyLinkList<ISystem> Systems { get; }
		IReadOnlyLinkList<IFamily> Families { get; }

		bool HasEntity(string globalName);
		bool HasEntity(IEntity entity);

		IEntity GetEntity(string globalName);

		Signal<IEngine, IEntity> EntityAdded { get; }
		Signal<IEngine, IEntity> EntityRemoved { get; }

		bool HasSystem(ISystem system);
		bool HasSystem<T>() where T : ISystem;
		bool HasSystem(Type type);

		TType GetSystem<TType>() where TType : ISystem;
		ISystem GetSystem(Type type);
		ISystem GetSystem(int index);

		Signal<IEngine, Type> SystemAdded { get; }
		Signal<IEngine, Type> SystemRemoved { get; }

		ISystem CurrentSystem { get; }

		bool HasFamily(IFamily family);
		bool HasFamily(Type type);
		bool HasFamily<TType>() where TType : IFamilyType;

		IFamily GetFamily(Type type);
		IFamily GetFamily<TType>() where TType : IFamilyType;

		IFamily AddFamily<TType>() where TType : IFamilyType;
		IFamily AddFamily(Type type);

		IFamily RemoveFamily<TType>() where TType : IFamilyType;
		IFamily RemoveFamily(Type type);

		Signal<IEngine, Type> FamilyAdded { get; }
		Signal<IEngine, Type> FamilyRemoved { get; }
	}
}
