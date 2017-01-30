using Atlas.Engine.Collections.Fixed;
using Atlas.Engine.Collections.LinkList;
using Atlas.Engine.Entities;
using Atlas.Engine.Families;
using Atlas.Engine.Signals;
using Atlas.Engine.Systems;
using System;

namespace Atlas.Engine.Components
{
	interface IEngine:IComponent
	{
		FixedStack<IEntity> EntityPool { get; }
		FixedStack<IFamily> FamilyPool { get; }

		IReadOnlyLinkList<IEntity> Entities { get; }
		IReadOnlyLinkList<ISystem> Systems { get; }
		IReadOnlyLinkList<IFamily> Families { get; }

		bool HasEntity(string globalName);
		bool HasEntity(IEntity entity);

		IEntity GetEntity(bool managed = true, string globalName = "", string localName = "");
		IEntity GetEntity(string globalName);

		ISignal<IEngine, IEntity> EntityAdded { get; }
		ISignal<IEngine, IEntity> EntityRemoved { get; }

		bool HasSystem(ISystem system);
		bool HasSystem<TSystem>() where TSystem : ISystem;
		bool HasSystem(Type type);

		TSystem GetSystem<TSystem>() where TSystem : ISystem;
		ISystem GetSystem(Type type);
		ISystem GetSystem(int index);

		ISignal<IEngine, Type> SystemAdded { get; }
		ISignal<IEngine, Type> SystemRemoved { get; }

		ISystem CurrentUpdateSystem { get; }
		ISystem CurrentFixedUpdateSystem { get; }

		bool HasFamily(IFamily family);
		bool HasFamily<TFamilyType>();
		bool HasFamily(Type type);

		IFamily GetFamily<TFamilyType>();
		IFamily GetFamily(Type type);

		IFamily AddFamily<TFamilyType>();
		IFamily AddFamily(Type type);

		IFamily RemoveFamily<TFamilyType>();
		IFamily RemoveFamily(Type type);

		ISignal<IEngine, Type> FamilyAdded { get; }
		ISignal<IEngine, Type> FamilyRemoved { get; }

		/// <summary>
		/// The delta time since the last <see cref="ISystem.Update"/> loop was started.
		/// </summary>
		float DeltaUpdateTime { get; }

		/// <summary>
		/// The total time spent running <see cref="ISystem.Update"/> loops was started.
		/// </summary>
		float TotalUpdateTime { get; }

		float DeltaFixedUpdateTime { get; set; }

		float TotalFixedUpdateTime { get; }

		float DeltaEngineTime { get; }

		float TotalEngineTime { get; }

		bool IsRunning { get; }

		void Run();

		bool IsUpdating { get; }

		ISignal<IEngine, bool> IsUpdatingChanged { get; }
	}
}