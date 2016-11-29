using Atlas.Engine.Components;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Interfaces;
using Atlas.Engine.LinkList;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Engine
{
	interface IEngineManager:IComponent, IUpdate<IEngineManager>, ISleep<IEngineManager>
	{
		Type DefaultEntity { get; set; }
		Type DefaultFamily { get; set; }

		bool IsEntityPool { get; set; }

		IReadOnlyLinkList<IEntity> Entities { get; }
		IReadOnlyLinkList<ISystem> Systems { get; }
		IReadOnlyLinkList<IFamily> Families { get; }

		IEntity RequestEntity(string globalName = "", string localName = "");

		bool HasEntity(string globalName);
		bool HasEntity(IEntity entity);

		IEntity GetEntity(string globalName);

		ISignal<IEngineManager, IEntity> EntityAdded { get; }
		ISignal<IEngineManager, IEntity> EntityRemoved { get; }

		bool HasSystem(ISystem system);
		bool HasSystem<TSystem>() where TSystem : ISystem;
		bool HasSystem(Type type);

		TSystem GetSystem<TSystem>() where TSystem : ISystem;
		ISystem GetSystem(Type type);
		ISystem GetSystem(int index);

		ISignal<IEngineManager, Type> SystemAdded { get; }
		ISignal<IEngineManager, Type> SystemRemoved { get; }

		ISystem CurrentSystem { get; }

		bool HasFamily(IFamily family);
		bool HasFamily<TFamilyType>() where TFamilyType : class;
		bool HasFamily(Type type);

		IFamily GetFamily<TFamilyType>() where TFamilyType : class;
		IFamily GetFamily(Type type);

		IFamily AddFamily<TFamilyType>() where TFamilyType : class;
		IFamily AddFamily(Type type);

		IFamily RemoveFamily<TFamilyType>() where TFamilyType : class;
		IFamily RemoveFamily(Type type);

		ISignal<IEngineManager, Type> FamilyAdded { get; }
		ISignal<IEngineManager, Type> FamilyRemoved { get; }
	}
}