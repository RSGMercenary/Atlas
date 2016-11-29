using Atlas.Engine.Signals;

namespace Atlas.Engine.Interfaces
{
	interface IUpdate<T>
	{
		void Update();
		bool IsUpdating { get; }
		ISignal<T, bool> IsUpdatingChanged { get; }
	}
}
