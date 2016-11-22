using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Systems;
using Atlas.Families;
using Atlas.Interfaces;
using Atlas.LinkList;
using Atlas.Signals;
using System;

namespace Atlas.Engine.Engine
{
	interface IEngineManager:IComponent, IUpdate<IEngineManager>
	{
		IReadOnlyLinkList<IEntity> Entities { get; }
		IReadOnlyLinkList<ISystem> Systems { get; }
		IReadOnlyLinkList<IFamily> Families { get; }

		bool HasEntity(string globalName);
		bool HasEntity(IEntity entity);

		IEntity GetEntity(string globalName);

		ISignal<IEngineManager, IEntity> EntityAdded { get; }
		ISignal<IEngineManager, IEntity> EntityRemoved { get; }

		bool HasSystem(ISystem system);
		bool HasSystem<T>() where T : ISystem;
		bool HasSystem(Type type);

		TType GetSystem<TType>() where TType : ISystem;
		ISystem GetSystem(Type type);
		ISystem GetSystem(int index);

		ISignal<IEngineManager, Type> SystemAdded { get; }
		ISignal<IEngineManager, Type> SystemRemoved { get; }

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

		ISignal<IEngineManager, Type> FamilyAdded { get; }
		ISignal<IEngineManager, Type> FamilyRemoved { get; }
	}
}
