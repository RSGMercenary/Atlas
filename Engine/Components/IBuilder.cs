using Atlas.Engine.Signals;

namespace Atlas.Engine.Components
{
	interface IBuilder:IComponent
	{
		/// <summary>
		/// Whether this Builder is currently building.
		/// </summary>
		bool IsBuilding { get; }

		/// <summary>
		/// Whether this Builder is currently built.
		/// </summary>
		bool IsBuilt { get; }

		/// <summary>
		/// Signal dispatching when this Builder has started and stopped building.
		/// </summary>
		ISignal<IBuilder, bool> IsBuildingChanged { get; }

		/// <summary>
		/// Signal dispatching when this Builder has been completely built.
		/// </summary>
		ISignal<IBuilder, bool> IsBuiltChanged { get; }
	}
}
